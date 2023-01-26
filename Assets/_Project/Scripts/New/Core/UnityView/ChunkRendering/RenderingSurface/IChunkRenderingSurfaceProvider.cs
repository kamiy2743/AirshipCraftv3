using Domain;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal interface IChunkRenderingSurfaceProvider
    {
        ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate);
    }
}