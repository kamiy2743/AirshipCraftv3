using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    public class UpdateChunkSystem
    {
        private Queue<ChunkCoordinate> waitingChunkQueue = new Queue<ChunkCoordinate>();

        private UniTask createFromWaitingQueueTask = UniTask.CompletedTask;
        private CancellationToken _cancellationToken;

        private ChunkDataStore _chunkDataStore;
        private ChunkObjectStore _chunkObjectStore;

        internal UpdateChunkSystem(Transform player, ChunkDataStore chunkDataStore, ChunkObjectStore chunkObjectStore)
        {
            _chunkDataStore = chunkDataStore;
            _chunkObjectStore = chunkObjectStore;

            var playerChunk = ChunkCoordinate.FromBlockCoordinate(new BlockCoordinate(player.position));
            UpdateAroundPlayer(playerChunk);
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private void UpdateAroundPlayer(ChunkCoordinate pc)
        {
            var createdChunkList = _chunkDataStore.Chunks.Keys.ToList();

            // 中心
            EnqueueLoadChunkHelper(pc.x, pc.z, pc.y, createdChunkList);

            // 内側から順に作成
            for (int r = 1; r <= WorldSettings.LoadChunkRadius; r++)
            {
                // 上から見てx+方向
                for (int x = pc.x - r; x < pc.x + r; x++)
                {
                    EnqueueLoadChunkHelper(x, pc.z + r, pc.y, createdChunkList);
                }
                // z-方向
                for (int z = pc.z + r; z > pc.z - r; z--)
                {
                    EnqueueLoadChunkHelper(pc.x + r, z, pc.y, createdChunkList);
                }
                // x-方向
                for (int x = pc.x + r; x > pc.x - r; x--)
                {
                    EnqueueLoadChunkHelper(x, pc.z - r, pc.y, createdChunkList);
                }
                // z+方向
                for (int z = pc.z - r; z < pc.z + r; z++)
                {
                    EnqueueLoadChunkHelper(pc.x - r, z, pc.y, createdChunkList);
                }
            }
        }

        /// <summary>
        /// 作成していないチャンクならキューに追加
        /// </summary>
        private void EnqueueLoadChunkHelper(int x, int z, int playerChunkY, IReadOnlyList<ChunkCoordinate> createdChunkList)
        {
            var radius = WorldSettings.LoadChunkRadius;

            // 下から順に作成
            for (int y = playerChunkY - radius; y <= playerChunkY + radius; y++)
            {
                if (!ChunkCoordinate.IsValid(x, y, z)) continue;

                var cc = new ChunkCoordinate(x, y, z);
                if (createdChunkList.Contains(cc)) continue;

                waitingChunkQueue.Enqueue(cc);

                // タスクを開始していなければ開始
                if (createFromWaitingQueueTask.Equals(UniTask.CompletedTask))
                {
                    createFromWaitingQueueTask = CreateFromWaitingQueue();
                    createFromWaitingQueueTask.Forget();
                }
            }
        }

        /// <summary>
        /// キューにあるチャンクを順次作成
        /// </summary>
        private async UniTask CreateFromWaitingQueue()
        {
            while (waitingChunkQueue.Count > 0)
            {
                var cc = waitingChunkQueue.Peek();
                var chunkData = _chunkDataStore.GetChunkData(cc);
                var chunkObject = _chunkObjectStore.CreateChunkObject(chunkData);
                waitingChunkQueue.Dequeue();
                await UniTask.DelayFrame(1, cancellationToken: _cancellationToken);
            }

            // タスク終了
            createFromWaitingQueueTask = UniTask.CompletedTask;
        }
    }
}
