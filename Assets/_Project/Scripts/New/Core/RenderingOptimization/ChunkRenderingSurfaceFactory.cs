using Domain.Chunks;

namespace RenderingOptimization
{
    internal class ChunkRenderingSurfaceFactory : IChunkRenderingSurfaceFactory
    {
        public ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var surfaces = new BlockRenderingSurfaces();

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        surfaces.SetBlockRenderingSurfaceDirectly(rc, new BlockRenderingSurface());
                    }
                }
            }

            return new ChunkRenderingSurface(chunkGridCoordinate, surfaces);
        }
    }
}