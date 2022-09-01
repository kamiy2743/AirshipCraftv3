using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldSystem;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    public class UpdateChunkSystem : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private ChunkDataStore chunkDataStore;

        private ChunkCoordinate lastPlayerChunk;

        private Queue<ChunkCoordinate> waitingChunkQueue = new Queue<ChunkCoordinate>();

        private UniTask createFromWaitingQueueTask = UniTask.CompletedTask;
        private CancellationToken _cancellationToken;

        void Start()
        {
            _cancellationToken = this.GetCancellationTokenOnDestroy();

            lastPlayerChunk = ChunkCoordinate.FromBlockCoordinate(new BlockCoordinate(player.position));
            UpdateAroundPlayer(lastPlayerChunk);
        }

        void Update()
        {
            // プレイヤーがチャンクをまたいだときのみ更新処理をする
            var playerChunk = ChunkCoordinate.FromBlockCoordinate(new BlockCoordinate(player.position));
            if (playerChunk == lastPlayerChunk) return;

            UpdateAroundPlayer(playerChunk);

            lastPlayerChunk = playerChunk;
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private void UpdateAroundPlayer(ChunkCoordinate pc)
        {
            var createdChunkList = chunkDataStore.Chunks.Keys.ToList();

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
                    // TODO エラーが出ない
                    createFromWaitingQueueTask = CreateFromWaitingQueue();
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
                var chunkData = chunkDataStore.GetChunkData(cc);
                var chunkObject = chunkDataStore.CreateChunkObject(chunkData);
                waitingChunkQueue.Dequeue();
                await UniTask.DelayFrame(1, cancellationToken: _cancellationToken);
            }

            // タスク終了
            createFromWaitingQueueTask = UniTask.CompletedTask;
        }
    }
}
