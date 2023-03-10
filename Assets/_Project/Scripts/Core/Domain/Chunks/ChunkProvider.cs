using System.Collections.Generic;

namespace Domain.Chunks
{
    internal class ChunkProvider : IChunkProvider
    {
        private IChunkFactory chunkFactory;
        private IChunkRepository chunkRepository;

        private Dictionary<ChunkGridCoordinate, Chunk> chunkCache = new Dictionary<ChunkGridCoordinate, Chunk>();

        internal ChunkProvider(IChunkFactory chunkFactory, IChunkRepository chunkRepository)
        {
            this.chunkFactory = chunkFactory;
            this.chunkRepository = chunkRepository;
        }

        public Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (chunkCache.TryGetValue(chunkGridCoordinate, out var cache))
            {
                return cache;
            }

            try
            {
                var chunk = chunkRepository.Fetch(chunkGridCoordinate);
                chunkCache.Add(chunkGridCoordinate, chunk);
                return chunk;
            }
            catch
            {
                var chunk = chunkFactory.Create(chunkGridCoordinate);
                chunkCache.Add(chunkGridCoordinate, chunk);
                return chunk;
            }
        }
    }
}