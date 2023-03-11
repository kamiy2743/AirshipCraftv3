using System.Collections.Generic;
using Domain;
using UnityView.Rendering.Chunks;

namespace Infrastructure
{
    class OnMemoryChunkSurfaceRepository : IChunkSurfaceRepository
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