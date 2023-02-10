using Domain;

namespace UnityView.Rendering.Chunks
{
    public interface IChunkSurfaceRepository
    {
        void Store(ChunkSurface chunkSurface);
        
        ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}