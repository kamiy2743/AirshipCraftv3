namespace Domain.Chunks
{
    public interface IChunkRepository
    {
        void Store(Chunk chunk);

        /// <exception cref="KeyNotFoundException"></exception>
        Chunk Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}