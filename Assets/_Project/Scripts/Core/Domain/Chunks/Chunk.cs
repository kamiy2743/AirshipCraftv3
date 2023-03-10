using System;

namespace Domain.Chunks
{
    public class Chunk : IEquatable<Chunk>
    {
        public readonly ChunkGridCoordinate chunkGridCoordinate;
        private readonly ChunkBlocks blocks;

        public const int BlockSideShift = 4;
        public const int BlockSide = 1 << BlockSideShift;
        public const int BlocksCount = BlockSide * BlockSide * BlockSide;

        internal Chunk(ChunkGridCoordinate chunkGridCoordinate, ChunkBlocks blocks)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
            this.blocks = blocks;
        }

        public Block GetBlock(RelativeCoordinate relativeCoordinate)
        {
            return blocks.GetBlock(relativeCoordinate);
        }

        public void SetBlock(RelativeCoordinate relativeCoordinate, Block block)
        {
            blocks.SetBlock(relativeCoordinate, block);
        }

        public Chunk DeepCopy()
        {
            return new Chunk(chunkGridCoordinate, blocks.DeepCopy());
        }

        public override bool Equals(object obj)
        {
            return obj is Chunk data && Equals(data);
        }

        public bool Equals(Chunk other)
        {
            return this.chunkGridCoordinate == other.chunkGridCoordinate;
        }

        public override int GetHashCode()
        {
            return chunkGridCoordinate.GetHashCode();
        }
    }
}