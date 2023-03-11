using System.Collections.Generic;
using ACv3.Domain;
using ACv3.UnityView.Rendering.Chunks;

namespace ACv3.Infrastructure
{
    public class OnMemoryChunkSurfaceRepository : IChunkSurfaceRepository
    {
        internal Dictionary<ChunkGridCoordinate, ChunkSurface> surfaces = new();

        public void Store(ChunkSurface chunkSurface)
        {
            surfaces[chunkSurface.chunkGridCoordinate] = chunkSurface.DeepCopy();
        }

        public ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return surfaces[chunkGridCoordinate].DeepCopy();
        }
    }
}