using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

namespace BlockSystem
{
    internal class ChunkData
    {
        internal ChunkCoordinate ChunkCoordinate { get; private set; }

        internal BlockData[] Blocks;

        internal static readonly ChunkData Empty = new ChunkData();

        private ChunkData()
        {
            ChunkCoordinate = new ChunkCoordinate(0, 0, 0);
            Blocks = new BlockData[World.BlockCountInChunk];
            for (int i = 0; i < World.BlockCountInChunk; i++)
            {
                Blocks[i] = BlockData.Empty;
            }
        }

        internal ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;

            var job = new CreateBlockDataJob
            {
                chunkRoot = new int3(cc.x, cc.y, cc.z) * World.ChunkBlockSide,
                mapGenerator = mapGenerator,
                blocks = new NativeArray<BlockData>(World.BlockCountInChunk, Allocator.TempJob),
            };

            job.Schedule(World.BlockCountInChunk, 0).Complete();

            Blocks = job.blocks.ToArray();

            job.blocks.Dispose();
        }

        internal static int ToIndex(LocalCoordinate lc)
        {
            return (lc.y * World.ChunkBlockSide * World.ChunkBlockSide) + (lc.z * World.ChunkBlockSide) + lc.x;
        }

        internal void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            Blocks[ToIndex(lc)] = blockData;
        }

        internal BlockData GetBlockData(LocalCoordinate lc)
        {
            return Blocks[ToIndex(lc)];
        }

        /// <summary>
        /// コンストラクタ用
        /// BLockDataを生成する
        /// </summary>
        [BurstCompile]
        private struct CreateBlockDataJob : IJobParallelFor
        {
            public int3 chunkRoot;
            public MapGenerator mapGenerator;
            public NativeArray<BlockData> blocks;

            public void Execute(int index)
            {
                var localY = index / (World.ChunkBlockSide * World.ChunkBlockSide);
                var xz = index - (localY * (World.ChunkBlockSide * World.ChunkBlockSide));
                var localX = xz % World.ChunkBlockSide;
                var localZ = xz / World.ChunkBlockSide;

                var localCoordinate = new int3(localX, localY, localZ);
                var blockCoordinate = localCoordinate + chunkRoot;
                var blockID = mapGenerator.GetBlockID(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z);

                blocks[index] = new BlockData(blockID, new BlockCoordinate(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z));
            }
        }

    }
}
