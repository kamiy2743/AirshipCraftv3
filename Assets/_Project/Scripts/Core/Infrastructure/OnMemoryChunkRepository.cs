using Domain;
using Domain.Chunks;
using System.Collections.Generic;

namespace Infrastructure
{
    class OnMemoryChunkRepository : IChunkRepository
    {
        readonly Dictionary<ChunkGridCoordinate, Chunk> _chunks = new Dictionary<ChunkGridCoordinate, Chunk>();

        public void Store(Chunk chunk)
        {
            _chunks[chunk.ChunkGridCoordinate] = chunk.DeepCopy();
        }

        public Chunk Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return _chunks[chunkGridCoordinate].DeepCopy();
        }
    }
}