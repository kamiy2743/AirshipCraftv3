using Domain;

namespace UnityView.ChunkRender
{
    public interface IChunkSurfaceRepository
    {
        void Store(ChunkSurface chunkSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}