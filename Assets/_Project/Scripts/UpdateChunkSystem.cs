using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクを更新する
    /// </summary>
    public class UpdateChunkSystem : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private ChunkDataStore chunkDataStore;

        private ChunkCoordinate lastPlayerChunk;

        private Queue<ChunkCoordinate> loadChunkQueue = new Queue<ChunkCoordinate>();

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
        /// プレイヤーのいるチャンクを中心とする立方体を更新する
        /// </summary>
        private void UpdateAroundPlayer(ChunkCoordinate pc)
        {
            var loadedChunkList = chunkDataStore.LoadedChunks.Keys.ToList();
            var radius = WorldSettings.LoadChunkRadius;

            // 下から順に更新
            for (int y = -radius; y <= radius; y++)
            {
                // 中心
                EnqueueLoadChunkHelper(pc.x, y, pc.z, loadedChunkList);

                // 内側から順に更新
                for (int r = 1; r <= radius; r++)
                {
                    // 上から見てx+方向
                    for (int x = pc.x - r; x < pc.x + r; x++)
                    {
                        EnqueueLoadChunkHelper(x, y, pc.z + r, loadedChunkList);
                    }
                    // z-方向
                    for (int z = pc.z + r; z > pc.z - r; z--)
                    {
                        EnqueueLoadChunkHelper(pc.x + r, y, z, loadedChunkList);
                    }
                    // x-方向
                    for (int x = pc.x + r; x > pc.x - r; x--)
                    {
                        EnqueueLoadChunkHelper(x, y, pc.z - r, loadedChunkList);
                    }
                    // z+方向
                    for (int z = pc.z - r; z < pc.z + r; z++)
                    {
                        EnqueueLoadChunkHelper(pc.x - r, y, z, loadedChunkList);
                    }
                }
            }
        }

        /// <summary>
        /// 読み込みしていないチャンクならキューに追加
        /// </summary>
        private void EnqueueLoadChunkHelper(int x, int y, int z, IReadOnlyList<ChunkCoordinate> loadedChunkList)
        {
            if (!ChunkCoordinate.IsValid(x, y, z)) return;

            var cc = new ChunkCoordinate(x, y, z);
            if (!loadedChunkList.Contains(cc))
            {
                loadChunkQueue.Enqueue(cc);
                var chunkData = chunkDataStore.GetChunkData(cc);
                var chunkObject = chunkDataStore.CreateChunkObject(chunkData);
            }
        }
    }
}
