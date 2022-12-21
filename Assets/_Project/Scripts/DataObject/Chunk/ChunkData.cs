using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Util;
using DataObject.Block;
using UniRx;

namespace DataObject.Chunk
{
    public unsafe class ChunkData : IEquatable<ChunkData>, IDisposable
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }
        public BlockData[] Blocks { get; private set; }

        public readonly ReferenceCounter ReferenceCounter = new ReferenceCounter();

        public IObservable<Unit> OnUpdate => _onUpdate;
        private Subject<Unit> _onUpdate = new Subject<Unit>();

        private int hashCode = Guid.NewGuid().GetHashCode();

        public static readonly ChunkData Empty = ForEmpty();

        /// <summary>
        /// チャンク内を満たすブロックの立方体の一辺の長さ
        /// </summary>
        public const byte ChunkBlockSide = 1 << ChunkBlockSideShift;
        public const byte ChunkBlockSideShift = 4;

        /// <summary>
        /// チャンク内のブロックの総数
        /// </summary>
        public const int BlockCountInChunk = ChunkBlockSide * ChunkBlockSide * ChunkBlockSide;

        private static readonly MapGenerator MapGenerator = new MapGenerator(1024, 0.01f);

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
        public ChunkData ReuseDeserialization(ChunkCoordinate cc, BlockData[] blocks)
        {
            ChunkCoordinate = cc;
            Blocks = blocks;
            return this;
        }

        /// <summary>
        /// デシリアライズ用なのでそれ以外では使用しないでください
        /// 新規作成するのでアロケーションが発生します
        /// </summary>
        public static ChunkData NewDeserialization(ChunkCoordinate cc, BlockData[] blocks)
        {
            return new ChunkData().ReuseDeserialization(cc, blocks);
        }

        /// <summary>
        /// 通常のコンストラクタです
        /// 新規作成するのでアロケーションが発生します
        /// </summary>
        public static unsafe ChunkData NewConstructor(ChunkCoordinate cc)
        {
            var chunkData = new ChunkData();
            chunkData.Blocks = new BlockData[BlockCountInChunk];
            return chunkData.ReuseConstructor(cc);
        }

        /// <summary>
        /// フィールドのメモリを流用し、コンストラクタを実行します
        /// </summary>
        public ChunkData ReuseConstructor(ChunkCoordinate cc)
        {
            ChunkCoordinate = cc;
            SetupBlocks(cc, Blocks);
            return this;
        }

        /// <summary>
        /// BlocksをBlockDataで埋めます
        /// </summary>
        private static unsafe void SetupBlocks(ChunkCoordinate cc, BlockData[] blocks)
        {
            fixed (BlockData* blocksFirst = &blocks[0])
            {
                var job = new SetupBlocksJob
                {
                    blocksFirst = blocksFirst,
                    chunkRoot = new int3(cc.x, cc.y, cc.z) * ChunkBlockSide,
                    mapGenerator = MapGenerator
                };

                job.Schedule(BlockCountInChunk, 128).Complete();
            }
        }

        private static int ToIndex(LocalCoordinate lc)
        {
            return ToIndex(lc.x, lc.y, lc.z);
        }

        public static int ToIndex(int lcx, int lcy, int lcz)
        {
            return lcx + (lcy << ChunkBlockSideShift) + (lcz << (ChunkBlockSideShift * 2));
        }

        public void InvokeUpdateEvent()
        {
            _onUpdate.OnNext(Unit.Default);
        }

        public void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            Blocks[ToIndex(lc)] = blockData;
        }

        public BlockData GetBlockData(LocalCoordinate lc)
        {
            return Blocks[ToIndex(lc)];
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkData data && Equals(data);
        }

        public bool Equals(ChunkData other)
        {
            return this.hashCode == other.hashCode;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public static bool operator ==(ChunkData left, ChunkData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkData left, ChunkData right)
        {
            return !(left == right);
        }

        public void Dispose()
        {
            ReferenceCounter.Dispose();
        }

        /// <summary>
        /// Blocks生成用Job
        /// </summary>
        [BurstCompile]
        private unsafe struct SetupBlocksJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* blocksFirst;

            [ReadOnly] public int3 chunkRoot;
            [ReadOnly] public MapGenerator mapGenerator;

            public void Execute(int index)
            {
                var lcx = index & LocalCoordinate.ToLocalCoordinateMask;
                var lcy = (index >> ChunkBlockSideShift) & LocalCoordinate.ToLocalCoordinateMask;
                var lcz = (index >> (ChunkBlockSideShift * 2)) & LocalCoordinate.ToLocalCoordinateMask;

                var bcx = chunkRoot.x + lcx;
                var bcy = chunkRoot.y + lcy;
                var bcz = chunkRoot.z + lcz;
                var blockID = mapGenerator.GetBlockID(bcx, bcy, bcz);

                *(blocksFirst + index) = new BlockData(blockID, new BlockCoordinate(bcx, bcy, bcz));
            }
        }
    }
}
