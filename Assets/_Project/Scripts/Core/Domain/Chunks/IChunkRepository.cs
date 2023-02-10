namespace Domain.Chunks
{
    public interface IChunkRepository
    {
        void Store(Chunk chunk);
        
        Chunk Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}