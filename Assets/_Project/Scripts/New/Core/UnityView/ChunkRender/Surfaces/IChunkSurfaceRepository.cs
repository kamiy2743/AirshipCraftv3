using Domain;

namespace UnityView.ChunkRender.Surfaces
{
    public interface IChunkSurfaceRepository
    {
        void Store(ChunkSurface chunkSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}