using System.Collections;
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

        void Start()
        {
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
            var radius = WorldSettings.LoadChunkRadius;

            // 下から順に更新
            for (int y = pc.y - radius; y <= pc.y + radius; y++)
            {
                // 中心
                EnqueueLoadChunkHelper(pc.x, y, pc.z, createdChunkList);

                // 内側から順に更新
                for (int r = 1; r <= radius; r++)
                {
                    // 上から見てx+方向
                    for (int x = pc.x - r; x < pc.x + r; x++)
                    {
                        EnqueueLoadChunkHelper(x, y, pc.z + r, createdChunkList);
                    }
                    // z-方向
                    for (int z = pc.z + r; z > pc.z - r; z--)
                    {
                        EnqueueLoadChunkHelper(pc.x + r, y, z, createdChunkList);
                    }
                    // x-方向
                    for (int x = pc.x + r; x > pc.x - r; x--)
                    {
                        EnqueueLoadChunkHelper(x, y, pc.z - r, createdChunkList);
                    }
                    // z+方向
                    for (int z = pc.z - r; z < pc.z + r; z++)
                    {
                        EnqueueLoadChunkHelper(pc.x - r, y, z, createdChunkList);
                    }
                }
            }
        }

        /// <summary>
        /// 作成していないチャンクならキューに追加
        /// </summary>
        private void EnqueueLoadChunkHelper(int x, int y, int z, IReadOnlyList<ChunkCoordinate> createdChunkList)
        {
            if (!ChunkCoordinate.IsValid(x, y, z)) return;

            var cc = new ChunkCoordinate(x, y, z);
            if (!createdChunkList.Contains(cc))
            {
                waitingChunkQueue.Enqueue(cc);
                if (waitingChunkQueue.Count == 1)
                {
                    CreateFromWaitingQueue().Forget();
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
                await UniTask.Delay(10);
                var chunkObject = chunkDataStore.CreateChunkObject(chunkData);
                waitingChunkQueue.Dequeue();
            }
        }
    }
}
