using Domain.Chunks;

namespace UseCase
{
    internal class ChunkProvider : IChunkProvider
    {
        private IChunkFactory chunkFactory;

        internal ChunkProvider(IChunkFactory chunkFactory)
        {
            this.chunkFactory = chunkFactory;
        }

        public Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate)
        {
            return chunkFactory.Create(chunkGridCoordinate);
        }
    }
}