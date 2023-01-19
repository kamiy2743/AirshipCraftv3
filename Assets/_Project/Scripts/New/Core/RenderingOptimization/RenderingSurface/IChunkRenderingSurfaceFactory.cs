using Domain.Chunks;

namespace RenderingOptimization.RenderingSurface
{
    internal interface IChunkRenderingSurfaceFactory
    {
        ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}