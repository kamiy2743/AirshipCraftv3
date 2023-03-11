using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace Infrastructure
{
    class OnMemoryChunkRepository : IChunkRepository
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