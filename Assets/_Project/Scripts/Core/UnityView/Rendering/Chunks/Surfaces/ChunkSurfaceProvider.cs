using System.Collections.Generic;
using Domain;

namespace UnityView.Rendering.Chunks
{
    class ChunkSurfaceProvider
    {
        readonly IChunkSurfaceRepository _chunkSurfaceRepository;
        readonly ChunkSurfaceFactory _chunkSurfaceFactory;

        readonly Dictionary<ChunkGridCoordinate, ChunkSurface> _surfaceCache = new Dictionary<ChunkGridCoordinate, ChunkSurface>();

        internal ChunkSurfaceProvider(IChunkSurfaceRepository chunkSurfaceRepository, ChunkSurfaceFactory chunkSurfaceFactory)
        {
            _chunkSurfaceRepository = chunkSurfaceRepository;
            _chunkSurfaceFactory = chunkSurfaceFactory;
        }

        internal ChunkSurface GetChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (_surfaceCache.TryGetValue(chunkGridCoordinate, out var cache))
            {
                return cache;
            }

            try
            {
                var surface = _chunkSurfaceRepository.Fetch(chunkGridCoordinate);

                _surfaceCache.Add(chunkGridCoordinate, surface);
                return surface;
            }
            catch
            {
                var surface = _chunkSurfaceFactory.Create(chunkGridCoordinate);
                _chunkSurfaceRepository.Store(surface);

                _surfaceCache.Add(chunkGridCoordinate, surface);
                return surface;
            }
        }
    }
}