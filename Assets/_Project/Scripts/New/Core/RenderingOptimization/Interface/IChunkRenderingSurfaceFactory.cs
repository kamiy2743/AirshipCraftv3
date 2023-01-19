using Domain.Chunks;

namespace RenderingOptimization
{
    internal interface IChunkRenderingSurfaceFactory
    {
        ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}