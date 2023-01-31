using Domain;

namespace UnityView.ChunkRender.Surfaces
{
    public interface IChunkRenderingSurfaceRepository
    {
        void Store(ChunkRenderingSurface renderingSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkRenderingSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}