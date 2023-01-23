using Domain.Chunks;

namespace RenderingDomain.RenderingSurface
{
    internal interface IChunkRenderingSurfaceFactory
    {
        ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}