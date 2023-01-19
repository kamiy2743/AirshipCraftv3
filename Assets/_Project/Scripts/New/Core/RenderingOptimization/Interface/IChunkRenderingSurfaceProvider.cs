using Domain.Chunks;

namespace RenderingOptimization
{
    internal interface IChunkRenderingSurfaceProvider
    {
        ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate);
    }
}