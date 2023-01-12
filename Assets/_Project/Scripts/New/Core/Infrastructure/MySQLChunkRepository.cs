using Domain.Chunks;

namespace Infrastructure
{
    internal class MySQLChunkRepository : IChunkRepository
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