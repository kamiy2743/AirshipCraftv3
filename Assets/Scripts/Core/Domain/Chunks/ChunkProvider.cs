using System.Collections.Generic;

namespace ACv3.Domain.Chunks
{
    public class ChunkProvider : IChunkProvider
    {
        readonly IChunkFactory chunkFactory;
        readonly IChunkRepository chunkRepository;

        readonly Dictionary<ChunkGridCoordinate, Chunk> chunkCache = new();

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