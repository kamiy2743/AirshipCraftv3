using Unity.Mathematics;
using Domain;
using Domain.Chunks;

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
            if (!BlockGridCoordinate.TryParse(position, out var placeCoordinate)) return;

            var block = new Block(blockTypeID);

            var cgc = ChunkGridCoordinate.Parse(placeCoordinate);
            var rc = RelativeCoordinate.Parse(placeCoordinate);
            var chunk = chunkProvider.GetChunk(cgc);
            chunk.SetBlock(rc, block);
            chunkRepository.Store(chunk);
        }
    }
}