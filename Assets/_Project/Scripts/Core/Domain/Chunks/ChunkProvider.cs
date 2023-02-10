using System.Collections.Generic;

namespace Domain.Chunks
{
    class ChunkProvider : IChunkProvider
    {
        readonly IChunkFactory _chunkFactory;
        readonly IChunkRepository _chunkRepository;

        readonly Dictionary<ChunkGridCoordinate, Chunk> _chunkCache = new Dictionary<ChunkGridCoordinate, Chunk>();

        internal ChunkProvider(IChunkFactory chunkFactory, IChunkRepository chunkRepository)
        {
            _chunkFactory = chunkFactory;
            _chunkRepository = chunkRepository;
        }

        public Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (_chunkCache.TryGetValue(chunkGridCoordinate, out var cache))
            {
                return cache;
            }

            try
            {
                var chunk = _chunkRepository.Fetch(chunkGridCoordinate);
                _chunkCache.Add(chunkGridCoordinate, chunk);
                return chunk;
            }
            catch
            {
                var chunk = _chunkFactory.Create(chunkGridCoordinate);
                _chunkCache.Add(chunkGridCoordinate, chunk);
                return chunk;
            }
        }
    }
}