using Unity.Mathematics;
using Domain;
using Domain.Chunks;

namespace UseCase
{
    public class PlaceBlockUseCase
    {
        private ChunkBlockSetter chunkBlockSetter;

        internal PlaceBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            this.chunkBlockSetter = chunkBlockSetter;
        }

        // TODO 設置するブロックのIDをViewから受け取るのはおかしい
        public void PlaceBlock(float3 position, BlockTypeID blockTypeID)
        {
            if (BlockGridCoordinate.TryParse(position, out var placeCoordinate))
            {
                var block = new Block(blockTypeID);
                chunkBlockSetter.SetBlock(placeCoordinate, block);
            }
        }
    }
}