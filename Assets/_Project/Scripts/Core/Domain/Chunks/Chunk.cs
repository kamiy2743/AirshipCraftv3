using System;

namespace Domain.Chunks
{
    public class Chunk : IEquatable<Chunk>
    {
        public readonly ChunkGridCoordinate ChunkGridCoordinate;
        readonly ChunkBlocks _blocks;

        public const int BlockSideShift = 4;
        public const int BlockSide = 1 << BlockSideShift;
        public const int BlocksCount = BlockSide * BlockSide * BlockSide;

        internal Chunk(ChunkGridCoordinate chunkGridCoordinate, ChunkBlocks blocks)
        {
            ChunkGridCoordinate = chunkGridCoordinate;
            _blocks = blocks;
        }

        public Block GetBlock(RelativeCoordinate relativeCoordinate)
        {
            return _blocks.GetBlock(relativeCoordinate);
        }

        public void SetBlock(RelativeCoordinate relativeCoordinate, Block block)
        {
            _blocks.SetBlock(relativeCoordinate, block);
        }

        public Chunk DeepCopy()
        {
            return new Chunk(ChunkGridCoordinate, _blocks.DeepCopy());
        }

        public override bool Equals(object obj)
        {
            return obj is Chunk data && Equals(data);
        }

        public bool Equals(Chunk other)
        {
            return other != null && ChunkGridCoordinate == other.ChunkGridCoordinate;
        }

        public override int GetHashCode()
        {
            return ChunkGridCoordinate.GetHashCode();
        }
    }
}