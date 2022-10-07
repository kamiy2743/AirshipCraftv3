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

        /// <summary>チャンク内を満たすブロックの立方体の一辺の長さ</summary>
        public const byte ChunkBlockSide = 1 << ChunkBlockSideShift;
        internal const byte ChunkBlockSideShift = 4;
        /// <summary>チャンク内のブロックの総数</summary>
        internal const int BlockCountInChunk = ChunkBlockSide * ChunkBlockSide * ChunkBlockSide;

        /// <summary>
        /// Empty用
        /// </summary>
        private ChunkData()
        {
            ChunkCoordinate = new ChunkCoordinate(0, 0, 0);
            Blocks = new BlockData[BlockCountInChunk];
            for (int i = 0; i < BlockCountInChunk; i++)
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
            Blocks = new BlockData[BlockCountInChunk];

            fixed (BlockData* blocksFirst = &Blocks[0])
            {
                var job = new CreateBlockDataJob
                {
                    blocksFirst = blocksFirst,
                    chunkRoot = new int3(cc.x, cc.y, cc.z) * ChunkBlockSide,
                    mapGenerator = mapGenerator
                };

                job.Schedule(BlockCountInChunk, 0).Complete();
            }

        }

        internal static int ToIndex(LocalCoordinate lc)
        {
            return (lc.y * ChunkBlockSide * ChunkBlockSide) + (lc.z * ChunkBlockSide) + lc.x;
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
                var localY = (int)math.floor((float)index / (ChunkBlockSide * ChunkBlockSide));
                var xz = index - (localY * (ChunkBlockSide * ChunkBlockSide));
                var localX = (int)math.floor((float)xz % ChunkBlockSide);
                var localZ = (int)math.floor((float)xz / ChunkBlockSide);

                var blockCoordinate = chunkRoot + new int3(localX, localY, localZ);
                var blockID = mapGenerator.GetBlockID(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z);

                *(blocksFirst + index) = new BlockData(blockID, new BlockCoordinate(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z));
            }
        }

    }
}
