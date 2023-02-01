using Domain;
using Domain.Chunks;

namespace UseCase
{
    internal class ChunkBlockSetter
    {
        private IChunkRepository chunkRepository;
        private IChunkProvider chunkProvider;

        internal ChunkBlockSetter(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            this.chunkRepository = chunkRepository;
            this.chunkProvider = chunkProvider;
        }

        internal void SetBlock(BlockGridCoordinate coordinate, Block block)
        {
            var cgc = ChunkGridCoordinate.Parse(coordinate);
            var rc = RelativeCoordinate.Parse(coordinate);
            var chunk = chunkProvider.GetChunk(cgc);
            chunk.SetBlock(rc, block);

            chunkRepository.Store(chunk);
        }
    }
}