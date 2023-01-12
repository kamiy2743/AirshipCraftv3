namespace Domain.Chunks
{
    public interface IChunkFactory
    {
        Chunk Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}