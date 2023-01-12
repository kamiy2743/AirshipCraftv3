using Unity.Mathematics;

namespace Domain.Chunks
{
    internal class ChunkBlocks
    {
        private readonly Block[] blocks;

        internal ChunkBlocks()
        {
            blocks = new Block[(int)math.pow(Chunk.BlockSide, 3)];
        }

        private ChunkBlocks(Block[] blocks)
        {
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

        private int RelativeCoordinateToIndex(RelativeCoordinate relativeCoordinate)
        {
            return (relativeCoordinate.x << (Chunk.BlockSideShift * 2)) + (relativeCoordinate.y << Chunk.BlockSideShift) + relativeCoordinate.z;
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