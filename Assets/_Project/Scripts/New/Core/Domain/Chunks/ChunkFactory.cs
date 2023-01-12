namespace Domain.Chunks
{
    public class ChunkFactory : IChunkFactory
    {
        public Chunk Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var blocks = new ChunkBlocks();

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        blocks.SetBlock(rc, new Block(BlockTypeID.Dirt));
                    }
                }
            }

            return new Chunk(chunkGridCoordinate, blocks);
        }
    }
}