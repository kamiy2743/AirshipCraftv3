using System.Collections.Generic;
using Domain;

namespace UnityView.Render.Chunks
{
    internal class ChunkSurfaceProvider
    {
        private IChunkSurfaceRepository chunkSurfaceRepository;
        private ChunkSurfaceFactory chunkSurfaceFactory;

        private Dictionary<ChunkGridCoordinate, ChunkSurface> surfaceCache = new Dictionary<ChunkGridCoordinate, ChunkSurface>();

        internal ChunkSurfaceProvider(IChunkSurfaceRepository chunkSurfaceRepository, ChunkSurfaceFactory chunkSurfaceFactory)
        {
            this.chunkSurfaceRepository = chunkSurfaceRepository;
            this.chunkSurfaceFactory = chunkSurfaceFactory;
        }

        internal ChunkSurface GetChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (surfaceCache.TryGetValue(chunkGridCoordinate, out var cache))
            {
                return cache;
            }

            try
            {
                var surface = chunkSurfaceRepository.Fetch(chunkGridCoordinate);

                surfaceCache.Add(chunkGridCoordinate, surface);
                return surface;
            }
            catch
            {
                var surface = chunkSurfaceFactory.Create(chunkGridCoordinate);
                chunkSurfaceRepository.Store(surface);

                surfaceCache.Add(chunkGridCoordinate, surface);
                return surface;
            }
        }
    }
}