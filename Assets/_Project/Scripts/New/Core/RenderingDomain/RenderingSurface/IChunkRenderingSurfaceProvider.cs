using Domain.Chunks;

namespace RenderingDomain.RenderingSurface
{
    internal interface IChunkRenderingSurfaceProvider
    {
        ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate);
    }
}