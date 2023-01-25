using Domain.Chunks;

namespace UnityView.ChunkRendering.Model.RenderingSurface
{
    internal interface IChunkRenderingSurfaceFactory
    {
        ChunkRenderingSurface Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}