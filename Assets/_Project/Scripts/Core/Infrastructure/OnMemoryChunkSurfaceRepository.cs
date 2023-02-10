using System.Collections.Generic;
using Domain;
using UnityView.Rendering.Chunks;

namespace Infrastructure
{
    class OnMemoryChunkSurfaceRepository : IChunkSurfaceRepository
    {
        readonly Dictionary<ChunkGridCoordinate, ChunkSurface> _surfaces = new Dictionary<ChunkGridCoordinate, ChunkSurface>();

        public void Store(ChunkSurface chunkSurface)
        {
            _surfaces[chunkSurface.ChunkGridCoordinate] = chunkSurface.DeepCopy();
        }

        public ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return _surfaces[chunkGridCoordinate].DeepCopy();
        }
    }
}