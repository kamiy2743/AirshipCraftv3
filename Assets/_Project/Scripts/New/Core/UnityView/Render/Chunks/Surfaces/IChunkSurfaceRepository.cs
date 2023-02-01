using Domain;

namespace UnityView.Render.Chunks
{
    public interface IChunkSurfaceRepository
    {
        void Store(ChunkSurface chunkSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}