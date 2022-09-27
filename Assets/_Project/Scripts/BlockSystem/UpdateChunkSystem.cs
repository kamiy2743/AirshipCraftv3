using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    internal class UpdateChunkSystem
    {
        private CancellationTokenSource createMeshDataTaskCancellationTokenSource;

        private ChunkObjectPool _chunkObjectPool;
        private ChunkDataStore _chunkDataStore;
        private ChunkMeshCreator _chunkMeshCreator;

        private Queue<ChunkCoordinate> createChunkMeshDataQueue = new Queue<ChunkCoordinate>();
        private Queue<KeyValuePair<ChunkCoordinate, ChunkMeshData>> createChunkObjectQueue = new Queue<KeyValuePair<ChunkCoordinate, ChunkMeshData>>();

        internal UpdateChunkSystem(Transform player, ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkObjectPool = chunkObjectPool;
            _chunkDataStore = chunkDataStore;
            _chunkMeshCreator = chunkMeshCreator;

            // 初回の更新
            Vector3Int lastPlayerChunk = GetPlayerChunk(player.position);
            UpdateAroundPlayer(lastPlayerChunk);

            // 毎フレーム監視
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    // プレイヤーチャンクが変化したら更新
                    var playerChunk = GetPlayerChunk(player.position);
                    if (playerChunk != lastPlayerChunk)
                    {
                        UpdateAroundPlayer(playerChunk);
                        lastPlayerChunk = playerChunk;
                    }

                    // 別スレッドで作成したメッシュデータからChunkObjectを作成
                    while (createChunkObjectQueue.Count > 0)
                    {
                        var item = createChunkObjectQueue.Dequeue();
                        var chunkObject = _chunkObjectPool.TakeChunkObject(item.Key);
                        chunkObject.SetMesh(item.Value);
                    }
                });
        }

        /// <summary>
        /// プレイヤーがいるチャンクに変換
        /// </summary>
        private const float InverseBlockSide = 1f / World.ChunkBlockSide;
        private Vector3Int GetPlayerChunk(Vector3 playerPosition)
        {
            return new Vector3Int(
                // BlockSideで割り算
                (int)(playerPosition.x * InverseBlockSide),
                (int)(playerPosition.y * InverseBlockSide),
                (int)(playerPosition.z * InverseBlockSide)
            );
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private void UpdateAroundPlayer(Vector3Int pc)
        {
            // タスク実行中であればキャンセル
            createMeshDataTaskCancellationTokenSource?.Cancel();
            createMeshDataTaskCancellationTokenSource?.Dispose();
            createChunkMeshDataQueue.Clear();
            createChunkObjectQueue.Clear();

            // 読みこみ範囲外のチャンクオブジェクトを解放する
            foreach (var cc in _chunkObjectPool.ChunkObjects.Keys.ToList())
            {
                if (math.abs(cc.x - pc.x) > World.LoadChunkRadius ||
                    math.abs(cc.y - pc.y) > World.LoadChunkRadius ||
                    math.abs(cc.z - pc.z) > World.LoadChunkRadius)
                {
                    _chunkObjectPool.ReleaseChunkObject(cc);
                }
            }

            // 作成済みチャンク
            var createdChunkHashSet = _chunkObjectPool.CreatedChunkHashSet;

            // 指定されたxzにあるチャンクを下から順に作成キューに追加する
            void EnqueueChunk(int x, int z)
            {
                // 下から順に追加
                var ye = math.min(pc.y + World.LoadChunkRadius, World.WorldChunkSideY - 1);
                for (int y = math.max(pc.y - World.LoadChunkRadius, 0); y <= ye; y++)
                {
                    var cc = new ChunkCoordinate(x, y, z, true);
                    // 作成済みならスキップ
                    if (createdChunkHashSet.Contains(cc)) continue;

                    createChunkMeshDataQueue.Enqueue(cc);
                }
            }

            if ((pc.x >= 0 && pc.x < World.WorldBlockSideXZ) &&
                (pc.z >= 0 && pc.z < World.WorldBlockSideXZ))
            {
                // 中心
                EnqueueChunk(pc.x, pc.z);
            }

            // 内側から順に作成
            for (int r = 1; r <= World.LoadChunkRadius; r++)
            {
                // 上から見てx+方向
                if (pc.z + r < World.WorldChunkSideXZ)
                {
                    var xe = math.min(pc.x + r, World.WorldChunkSideXZ);
                    for (int x = math.max(pc.x - r, 0); x < xe; x++)
                    {
                        EnqueueChunk(x, pc.z + r);
                    }
                }
                // z-方向
                if (pc.x + r < World.WorldChunkSideXZ)
                {
                    var ze = math.max(pc.z - r, -1);
                    for (int z = math.min(pc.z + r, World.WorldChunkSideXZ); z > ze; z--)
                    {
                        EnqueueChunk(pc.x + r, z);
                    }
                }
                // x-方向
                if (pc.z - r >= 0)
                {
                    var xe = math.max(pc.x - r, -1);
                    for (int x = math.min(pc.x + r, World.WorldChunkSideXZ); x > xe; x--)
                    {
                        EnqueueChunk(x, pc.z - r);
                    }
                }
                // z+方向
                if (pc.x - r >= 0)
                {
                    var ze = math.min(pc.z + r, World.WorldChunkSideXZ);
                    for (int z = math.max(pc.z - r, 0); z < ze; z++)
                    {
                        EnqueueChunk(pc.x - r, z);
                    }
                }
            }

            // タスク開始
            createMeshDataTaskCancellationTokenSource = new CancellationTokenSource();
            CreateMeshDataFromQueue(createMeshDataTaskCancellationTokenSource.Token).Forget();
        }

        /// <summary>
        /// キューにあるチャンクのメッシュを順次作成
        /// </summary>
        private async UniTask CreateMeshDataFromQueue(CancellationToken ct)
        {
            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            while (createChunkMeshDataQueue.Count > 0)
            {
                if (ct.IsCancellationRequested) return;

                var cc = createChunkMeshDataQueue.Dequeue();
                var chunkData = _chunkDataStore.GetChunkData(cc);
                var meshData = _chunkMeshCreator.CreateMeshData(chunkData);

                if (ct.IsCancellationRequested) return;

                createChunkObjectQueue.Enqueue(new KeyValuePair<ChunkCoordinate, ChunkMeshData>(cc, meshData));
            }

            // メインスレッドに戻す
            await UniTask.SwitchToMainThread(ct);
        }
    }
}