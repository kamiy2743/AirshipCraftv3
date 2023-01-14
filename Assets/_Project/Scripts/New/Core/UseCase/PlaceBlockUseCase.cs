using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    public class PlaceBlockUseCase
    {
        private SetBlockService setBlockService;

        internal PlaceBlockUseCase(SetBlockService setBlockService)
        {
            this.setBlockService = setBlockService;
        }

        public void PlaceBlock(float3 position)
        {
            if (!BlockGridCoordinate.TryParse(position, out var blockGridCoordinate)) return;

            var placeBlock = new Block(BlockTypeID.Dirt);
            setBlockService.SetBlock(blockGridCoordinate, placeBlock);
        }
    }
}