using System.Threading;
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

            // 読みこみ範囲外のチャンクオブジェクトを解放する
            foreach (var cc in _chunkObjectPool.ChunkObjects.Keys.ToList())
            {
                var distance = new Vector3Int(Mathf.Abs(cc.x - pc.x), Mathf.Abs(cc.y - pc.y), Mathf.Abs(cc.z - pc.z));
                if (distance.x > World.LoadChunkRadius || distance.y > World.LoadChunkRadius || distance.z > World.LoadChunkRadius)
                {
                    _chunkObjectPool.ReleaseChunkObject(cc);
                }
            }

            // 作成済みチャンクのリスト
            var createdChunkList = _chunkObjectPool.ChunkObjects.Keys.ToList();
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