using Domain.Chunks;
using System.Collections.Generic;

namespace Infrastructure
{
    internal class OnMemoryChunkRepository : IChunkRepository
    {
        internal Dictionary<ChunkGridCoordinate, Chunk> chunks = new Dictionary<ChunkGridCoordinate, Chunk>();

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