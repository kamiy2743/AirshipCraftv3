using System;

namespace ACv3.Domain.Chunks
{
    class ChunkBlocks
    {
        internal readonly Block[] blocks;

        internal ChunkBlocks()
        {
            blocks = new Block[Chunk.BlocksCount];
        }

        internal ChunkBlocks(Block[] blocks)
        {
            if (blocks.Length != Chunk.BlocksCount) throw new ArgumentException("");
            this.blocks = blocks;
        }

        internal Block GetBlock(RelativeCoordinate relativeCoordinate)
        {
            return blocks[RelativeCoordinateToIndex(relativeCoordinate)];
        }

        internal void SetBlock(RelativeCoordinate relativeCoordinate, Block block)
        {
            blocks[RelativeCoordinateToIndex(relativeCoordinate)] = block;
        }

        int RelativeCoordinateToIndex(RelativeCoordinate rc)
        {
            return (rc.x << (Chunk.BlockSideShift * 2)) + (rc.y << Chunk.BlockSideShift) + rc.z;
        }

        internal ChunkBlocks DeepCopy()
        {
            var blocksCopy = new Block[blocks.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocksCopy[i] = blocks[i].DeepCopy();
            }

            return new ChunkBlocks(blocksCopy);
        }
    }
}