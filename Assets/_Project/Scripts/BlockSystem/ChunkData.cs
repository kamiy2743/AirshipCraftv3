using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Util;

namespace BlockSystem
{
    /// <summary>
    /// チャンクの内部データ
    /// </summary>
    internal class ChunkData
    {
        internal ChunkCoordinate ChunkCoordinate { get; private set; }
        internal BlockData[] Blocks { get; private set; }

        internal readonly ReferenceCounter ReferenceCounter = new ReferenceCounter();

        internal static readonly ChunkData Empty = ForEmpty();

        /// <summary>チャンク内を満たすブロックの立方体の一辺の長さ</summary>
        internal const byte ChunkBlockSide = 1 << ChunkBlockSideShift;
        internal const byte ChunkBlockSideShift = 4;

        /// <summary>チャンク内のブロックの総数</summary>
        internal const int BlockCountInChunk = ChunkBlockSide * ChunkBlockSide * ChunkBlockSide;

        private ChunkData() { }
        private static ChunkData ForEmpty()
        {
            var chunkData = new ChunkData();
            chunkData.ChunkCoordinate = new ChunkCoordinate(0, 0, 0);
            chunkData.Blocks = new BlockData[BlockCountInChunk];
            for (int i = 0; i < BlockCountInChunk; i++)
            {
                chunkData.Blocks[i] = BlockData.Empty;
            }
            return chunkData;
        }

        /// <summary>
        /// デシリアライズ用なのでそれ以外では使用しないでください
        /// 既存のフィールドのメモリを流用します
        /// </summary>
        internal ChunkData ReuseDeserialization(ChunkCoordinate cc, BlockData[] blocks)
        {
            ChunkCoordinate = cc;
            Blocks = blocks;
            return this;
        }

        /// <summary>
        /// デシリアライズ用なのでそれ以外では使用しないでください
        /// 新規作成するのでアロケーションが発生します
        /// </summary>
        internal static ChunkData NewDeserialization(ChunkCoordinate cc, BlockData[] blocks)
        {
            return new ChunkData().ReuseDeserialization(cc, blocks);
        }

        /// <summary>
        /// 通常のコンストラクタです
        /// 新規作成するのでアロケーションが発生します
        /// </summary>
        unsafe internal static ChunkData NewConstructor(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            var chunkData = new ChunkData();
            chunkData.Blocks = new BlockData[BlockCountInChunk];
            return chunkData.ReuseConstructor(cc, mapGenerator);
        }

        /// <summary>
        /// フィールドのメモリを流用し、コンストラクタを実行します
        /// </summary>
        internal ChunkData ReuseConstructor(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            SetupBlocks(cc, Blocks, mapGenerator);
            return this;
        }

        /// <summary>
        /// BlocksをBlockDataで埋めます
        /// </summary>
        unsafe private static void SetupBlocks(ChunkCoordinate cc, BlockData[] blocks, MapGenerator mapGenerator)
        {
            fixed (BlockData* blocksFirst = &blocks[0])
            {
                var job = new SetupBlocksJob
                {
                    blocksFirst = blocksFirst,
                    chunkRoot = new int3(cc.x, cc.y, cc.z) * ChunkBlockSide,
                    mapGenerator = mapGenerator
                };

                job.Schedule(BlockCountInChunk, 128).Complete();
            }
        }

        internal static int ToIndex(LocalCoordinate lc)
        {
            return lc.x + (lc.y << ChunkBlockSideShift) + (lc.z << (ChunkBlockSideShift * 2));
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
        /// Blocks生成用Job
        /// </summary>
        [BurstCompile]
        unsafe private struct SetupBlocksJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public global::BlockSystem.BlockData* blocksFirst;

            [ReadOnly] public int3 chunkRoot;
            [ReadOnly] public MapGenerator mapGenerator;

            private const byte mask = (ChunkBlockSide - 1);

            public void Execute(int index)
            {
                var lcx = index & mask;
                var lcy = (index >> ChunkBlockSideShift) & mask;
                var lcz = (index >> (ChunkBlockSideShift * 2)) & mask;

                var blockCoordinate = chunkRoot + new int3(lcx, lcy, lcz);
                var blockID = mapGenerator.GetBlockID(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z);

                *(blocksFirst + index) = new BlockData(blockID, new BlockCoordinate(blockCoordinate.x, blockCoordinate.y, blockCoordinate.z));
            }
        }
    }
}
