using System.Threading;
using System.Collections.Concurrent;
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
        private CancellationTokenSource createChunkTaskCancellationTokenSource;

        private ChunkObjectPool _chunkObjectPool;
        private ChunkObjectCreator _chunkObjectCreator;

        internal UpdateChunkSystem(Transform player, ChunkObjectPool chunkObjectPool, ChunkObjectCreator chunkObjectCreator)
        {
            _chunkObjectPool = chunkObjectPool;
            _chunkObjectCreator = chunkObjectCreator;

            // 初回の更新
            if (!TryGetPlayerChunk(player.position, out ChunkCoordinate firstPlayerChunk))
            {
                throw new System.Exception("最初は必ずワールドの範囲内にいてください");
            }
            UpdateAroundPlayer(firstPlayerChunk);

            // 前回の更新時のプレイヤーチャンクを保持
            ChunkCoordinate lastPlayerChunk = firstPlayerChunk;

            // 毎フレーム監視
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    // プレイヤーチャンクが変化したら更新
                    if (!TryGetPlayerChunk(player.position, out ChunkCoordinate playerChunk)) return;
                    if (playerChunk == lastPlayerChunk) return;

                    UpdateAroundPlayer(playerChunk);
                    lastPlayerChunk = playerChunk;
                });
        }

        private bool TryGetPlayerChunk(Vector3 playerPosition, out ChunkCoordinate playerChunk)
        {
            if (!BlockCoordinate.IsValid(playerPosition))
            {
                playerChunk = default;
                UnityEngine.Debug.Log("チャンク生成範囲外です");
                return false;
            }

            playerChunk = ChunkCoordinate.FromBlockCoordinate(new BlockCoordinate(playerPosition));
            return true;
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private void UpdateAroundPlayer(ChunkCoordinate pc)
        {
            // タスク実行中であればキャンセル
            createChunkTaskCancellationTokenSource?.Cancel();
            createChunkTaskCancellationTokenSource?.Dispose();

            // 作成済みチャンク
            var createdChunkHashSet = _chunkObjectPool.CreatedChunkHashSet;

            // 読みこみ範囲外のチャンクオブジェクトを解放する
            foreach (var cc in createdChunkHashSet)
            {
                if (math.abs(cc.x - pc.x) > World.LoadChunkRadius ||
                    math.abs(cc.y - pc.y) > World.LoadChunkRadius ||
                    math.abs(cc.z - pc.z) > World.LoadChunkRadius)
                {
                    _chunkObjectPool.ReleaseChunkObject(cc);
                }
            }

            // 作成するチャンクのキュー
            var createChunkQueue = new ConcurrentQueue<ChunkCoordinate>();

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

                    createChunkQueue.Enqueue(cc);
                }
            }

            // 中心
            EnqueueChunk(pc.x, pc.z);

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
                    var ze = math.max(pc.z - r, 0);
                    for (int z = math.min(pc.z + r, World.WorldChunkSideXZ); z > ze; z--)
                    {
                        EnqueueChunk(pc.x + r, z);
                    }
                }
                // x-方向
                if (pc.z - r >= 0)
                {
                    var xe = math.max(pc.x - r, 0);
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
            createChunkTaskCancellationTokenSource = new CancellationTokenSource();
            CreateChunkFromQueue(createChunkQueue, createChunkTaskCancellationTokenSource.Token).Forget();
        }

        /// <summary>
        /// キューにあるチャンクを順次作成
        /// </summary>
        private async UniTask CreateChunkFromQueue(ConcurrentQueue<ChunkCoordinate> createChunkQueue, CancellationToken ct)
        {
            ChunkMeshData meshData = null;

            while (createChunkQueue.Count > 0)
            {
                if (!createChunkQueue.TryDequeue(out ChunkCoordinate cc))
                {
                    throw new System.Exception("failed");
                }

                var (_, newMeshData) = await _chunkObjectCreator.CreateChunkObject(cc, ct, meshData);
                meshData = newMeshData;
            }
        }
    }
}