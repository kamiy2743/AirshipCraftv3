using ACv3.Domain;
using ACv3.Domain.Chunks;
using Unity.Mathematics;

namespace ACv3.UseCase
{
    public class PlaceBlockUseCase
    {
        readonly ChunkBlockSetter chunkBlockSetter;

        internal PlaceBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            this.chunkBlockSetter = chunkBlockSetter;
        }

        // TODO 設置するブロックのIDをViewから受け取るのはおかしい
        public void PlaceBlock(float3 position, BlockType blockType)
        {
            if (BlockGridCoordinate.TryParse(position, out var placeCoordinate))
            {
                var block = new Block(blockType);
                chunkBlockSetter.SetBlock(placeCoordinate, block);
            }
        }
    }
}