using Domain.Chunks;

namespace UnityView.ChunkRendering.Model.RenderingSurface
{
    internal interface IChunkRenderingSurfaceProvider
    {
        ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate);
    }
}