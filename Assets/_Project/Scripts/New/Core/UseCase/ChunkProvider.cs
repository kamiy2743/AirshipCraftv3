using Domain.Chunks;

namespace UseCase
{
    internal class ChunkProvider : IChunkProvider
    {
        private IChunkFactory chunkFactory;
        private IChunkRepository chunkRepository;

        internal ChunkProvider(IChunkFactory chunkFactory, IChunkRepository chunkRepository)
        {
            this.chunkFactory = chunkFactory;
            this.chunkRepository = chunkRepository;
        }

        public Chunk GetChunk(ChunkGridCoordinate chunkGridCoordinate)
        {
            try
            {
                return chunkRepository.Fetch(chunkGridCoordinate);
            }
            catch
            {
                return chunkFactory.Create(chunkGridCoordinate);
            }
        }
    }
}