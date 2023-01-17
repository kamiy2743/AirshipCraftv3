using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    public class PlaceBlockUseCase
    {
        private IChunkRepository chunkRepository;
        private IChunkProvider chunkProvider;

        internal PlaceBlockUseCase(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            this.chunkRepository = chunkRepository;
            this.chunkProvider = chunkProvider;
        }

        public void PlaceBlock(float3 position, BlockTypeID blockTypeID)
        {
            if (!BlockGridCoordinate.TryParse(position, out var blockGridCoordinate)) return;

            var block = new Block(blockTypeID);

            var cgc = ChunkGridCoordinate.Parse(blockGridCoordinate);
            var rc = RelativeCoordinate.Parse(blockGridCoordinate);
            var chunk = chunkProvider.GetChunk(cgc);
            chunk.SetBlock(rc, block);
            chunkRepository.Store(chunk);
        }
    }
}