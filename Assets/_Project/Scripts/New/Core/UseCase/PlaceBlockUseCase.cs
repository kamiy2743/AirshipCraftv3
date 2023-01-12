using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    internal class PlaceBlockUseCase
    {
        private IChunkRepository chunkRepository;
        private IChunkProvider chunkProvider;

        internal PlaceBlockUseCase(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            this.chunkRepository = chunkRepository;
            this.chunkProvider = chunkProvider;
        }

        internal void PlaceBlock(float3 position)
        {
            if (!BlockGridCoordinate.TryParse(position, out var blockGridCoordinate)) return;

            var placeBlock = new Block(BlockTypeID.Dirt);

            var setBlockService = new SetBlockService(chunkRepository, chunkProvider);
            setBlockService.SetBlock(blockGridCoordinate, placeBlock);
        }
    }
}