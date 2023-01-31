using System.Collections.Generic;
using Domain;
using UnityView.ChunkRender;

namespace Infrastructure
{
    internal class OnMemoryChunkSurfaceRepository : IChunkSurfaceRepository
    {
        internal Dictionary<ChunkGridCoordinate, ChunkSurface> surfaces = new Dictionary<ChunkGridCoordinate, ChunkSurface>();

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