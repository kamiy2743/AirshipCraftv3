using ACv3.Domain;
using ACv3.Domain.Chunks;

namespace ACv3.Infrastructure
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