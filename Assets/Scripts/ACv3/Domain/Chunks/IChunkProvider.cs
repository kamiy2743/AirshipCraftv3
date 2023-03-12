namespace ACv3.Domain.Chunks
{
    public interface IChunkProvider
    {
        Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate);
    }
}