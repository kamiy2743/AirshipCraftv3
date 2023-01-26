using Domain.Chunks;

namespace UnityView.ChunkRendering.RenderingSurface
{
    public interface IChunkRenderingSurfaceRepository
    {
        void Store(ChunkRenderingSurface renderingSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkRenderingSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}