using System.Collections.Generic;
using ACv3.Domain;
using ACv3.Domain.Chunks;

namespace ACv3.Infrastructure
{
    public class OnMemoryChunkRepository : IChunkRepository
    {
        internal Dictionary<ChunkGridCoordinate, Chunk> chunks = new();

        public void Store(Chunk chunk)
        {
            chunks[chunk.chunkGridCoordinate] = chunk.DeepCopy();
        }

        public Chunk Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return chunks[chunkGridCoordinate].DeepCopy();
        }
    }
}