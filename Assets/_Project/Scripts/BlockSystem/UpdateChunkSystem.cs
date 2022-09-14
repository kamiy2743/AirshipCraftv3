using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    public class UpdateChunkSystem
    {
        private CancellationTokenSource createChunkTaskCancellationTokenSource;

        private ChunkDataStore _chunkDataStore;
        private ChunkObjectPool _chunkObjectStore;
        private ChunkMeshCreator _chunkMeshCreator;

        internal UpdateChunkSystem(Transform player, ChunkDataStore chunkDataStore, ChunkObjectPool chunkObjectStore, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkObjectStore = chunkObjectStore;
            _chunkMeshCreator = chunkMeshCreator;

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

            // 読みこみ範囲外のチャンクオブジェクトを解放する
            foreach (var cc in _chunkObjectStore.ChunkObjects.Keys.ToList())
            {
                var distance = new Vector3Int(Mathf.Abs(cc.x - pc.x), Mathf.Abs(cc.y - pc.y), Mathf.Abs(cc.z - pc.z));
                if (distance.x > World.LoadChunkRadius || distance.y > World.LoadChunkRadius || distance.z > World.LoadChunkRadius)
                {
                    _chunkObjectStore.ReleaseChunkObject(cc);
                }
            }

            // 作成済みチャンクのリスト
            var createdChunkList = _chunkObjectStore.ChunkObjects.Keys.ToList();
            // 作成するチャンクのキュー
            var createChunkQueue = new ConcurrentQueue<ChunkCoordinate>();

            // 指定されたxzにあるチャンクを下から順に作成キューに追加する
            void EnqueueChunk(int x, int z)
            {
                // 下から順に追加
                for (int y = pc.y - World.LoadChunkRadius; y <= pc.y + World.LoadChunkRadius; y++)
                {
                    if (!ChunkCoordinate.IsValid(x, y, z)) continue;

                    var cc = new ChunkCoordinate(x, y, z);
                    if (createdChunkList.Contains(cc)) continue;

                    createChunkQueue.Enqueue(cc);
                }
            }

            // 中心
            EnqueueChunk(pc.x, pc.z);

            // 内側から順に作成
            for (int r = 1; r <= World.LoadChunkRadius; r++)
            {
                // 上から見てx+方向
                for (int x = pc.x - r; x < pc.x + r; x++)
                {
                    EnqueueChunk(x, pc.z + r);
                }
                // z-方向
                for (int z = pc.z + r; z > pc.z - r; z--)
                {
                    EnqueueChunk(pc.x + r, z);
                }
                // x-方向
                for (int x = pc.x + r; x > pc.x - r; x--)
                {
                    EnqueueChunk(x, pc.z - r);
                }
                // z+方向
                for (int z = pc.z - r; z < pc.z + r; z++)
                {
                    EnqueueChunk(pc.x - r, z);
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
                // 別スレッドに退避
                await UniTask.SwitchToThreadPool();

                if (!createChunkQueue.TryDequeue(out ChunkCoordinate cc))
                {
                    throw new System.Exception("failed");
                }

                var chunkData = _chunkDataStore.GetChunkData(cc);
                meshData = _chunkMeshCreator.CreateMeshData(chunkData.Blocks, meshData);
                chunkData.SetChunkMeshData(meshData);

                // UnityApiを使う処理をするのでメインスレッドに戻す
                await UniTask.SwitchToMainThread(ct);

                var chunkObject = _chunkObjectStore.GetChunkObject(cc);
                chunkObject.SetMesh(meshData);
                meshData.Clear();
            }
        }
    }
}