using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using MessagePack;

namespace BlockSystem
{
    [MessagePackObject]
    public class ChunkData
    {
        [Key(0)]
        public readonly ChunkCoordinate ChunkCoordinate;

        [Key(1)]
        public readonly BlockData[] Blocks;

        internal static readonly ChunkData Empty = new ChunkData();

        /// <summary>
        /// Empty用
        /// </summary>
        public ChunkData()
        {
            ChunkCoordinate = new ChunkCoordinate(0, 0, 0);
            Blocks = new BlockData[World.BlockCountInChunk];
            for (int i = 0; i < World.BlockCountInChunk; i++)
            {
                Blocks[i] = BlockData.Empty;
            }
        }

        [SerializationConstructor]
        public ChunkData(ChunkCoordinate cc, BlockData[] blocks)
        {
            ChunkCoordinate = cc;
            Blocks = blocks;
        }

        unsafe internal ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            Blocks = new BlockData[World.BlockCountInChunk];

            fixed (BlockData* blocksFirst = &Blocks[0])
            {
                var job = new CreateBlockDataJob
                {
                    blocksFirst = blocksFirst,
                    chunkRoot = new int3(cc.x, cc.y, cc.z) * World.ChunkBlockSide,
                    mapGenerator = mapGenerator
                };

                job.Schedule(World.BlockCountInChunk, 0).Complete();
            }

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
        unsafe private struct CreateBlockDataJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;

            [ReadOnly]
            public int3 chunkRoot;
            [ReadOnly]
            public MapGenerator mapGenerator;

            public void Execute(int index)
            {
                var localY = index / (World.ChunkBlockSide * World.ChunkBlockSide);
                var xz = index - (localY * (World.ChunkBlockSide * World.ChunkBlockSide));
                var localX = xz % World.ChunkBlockSide;
                var localZ = xz / World.ChunkBlockSide;

                var localCoordinate = new int3(localX, localY, localZ);
                var blockCoordinate = localCoordinate + chunkRoot;
                var blockID = mapGenerator.GetBlockID(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z);

                *(blocksFirst + index) = new BlockData(blockID, new BlockCoordinate(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z));
            }
        }

    }
}
