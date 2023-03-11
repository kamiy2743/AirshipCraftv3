using Domain;
using Domain.Chunks;

namespace Infrastructure
{
    class MySQLChunkRepository : IChunkRepository
    {
        public void Store(Chunk chunk)
        {

        }

        public Chunk Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return null;
        }
    }
}