using Unity.Mathematics;

namespace Domain.Chunks
{
    internal class SnoiseChunkFactory : IChunkFactory
    {
        private SnoiseTerrainGenerator snoiseTerrainGenerator;

        internal SnoiseChunkFactory()
        {
            snoiseTerrainGenerator = new SnoiseTerrainGenerator(128, 0.01f);
        }

        public Chunk Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var blocks = new ChunkBlocks();
            var rootCoordinate = new int3(chunkGridCoordinate.x, chunkGridCoordinate.y, chunkGridCoordinate.z) * Chunk.BlockSide;

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        var blockTypeID = snoiseTerrainGenerator.GetBlockTypeID(rootCoordinate.x + x, rootCoordinate.y + y, rootCoordinate.z + z);
                        blocks.SetBlock(rc, new Block(blockTypeID));
                    }
                }
            }

            return new Chunk(chunkGridCoordinate, blocks);
        }
    }
}