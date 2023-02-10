using System;

namespace Domain.Chunks
{
    class ChunkBlocks
    {
        readonly Block[] _blocks;

        internal ChunkBlocks()
        {
            _blocks = new Block[Chunk.BlocksCount];
        }

        internal ChunkBlocks(Block[] blocks)
        {
            if (blocks.Length != Chunk.BlocksCount) throw new ArgumentException("");
            _blocks = blocks;
        }

        internal Block GetBlock(RelativeCoordinate relativeCoordinate)
        {
            return _blocks[RelativeCoordinateToIndex(relativeCoordinate)];
        }

        internal void SetBlock(RelativeCoordinate relativeCoordinate, Block block)
        {
            _blocks[RelativeCoordinateToIndex(relativeCoordinate)] = block;
        }

        static int RelativeCoordinateToIndex(RelativeCoordinate rc)
        {
            return (rc.X << (Chunk.BlockSideShift * 2)) + (rc.Y << Chunk.BlockSideShift) + rc.Z;
        }

        internal ChunkBlocks DeepCopy()
        {
            var blocksCopy = new Block[_blocks.Length];
            for (int i = 0; i < _blocks.Length; i++)
            {
                blocksCopy[i] = _blocks[i].DeepCopy();
            }

            return new ChunkBlocks(blocksCopy);
        }
    }
}