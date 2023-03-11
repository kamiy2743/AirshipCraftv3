namespace ACv3.Domain.Chunks
{
    public interface IChunkFactory
    {
        Chunk Create(ChunkGridCoordinate chunkGridCoordinate);
    }
}