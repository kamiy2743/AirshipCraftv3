using Domain;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal interface IChunkRenderingSurfaceFactory
    {
        ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}