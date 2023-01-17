using Domain.Chunks;

namespace UseCase
{
    internal interface IChunkProvider
    {
        Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate);
    }
}