namespace Domain.Chunks
{
    public interface IChunkProvider
    {
        Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate);
    }
}