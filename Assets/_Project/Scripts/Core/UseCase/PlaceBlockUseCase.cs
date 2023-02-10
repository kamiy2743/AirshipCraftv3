using Unity.Mathematics;
using Domain;
using Domain.Chunks;

namespace UseCase
{
    public class PlaceBlockUseCase
    {
        readonly ChunkBlockSetter _chunkBlockSetter;

        internal PlaceBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            _chunkBlockSetter = chunkBlockSetter;
        }

        // TODO 設置するブロックのIDをViewから受け取るのはおかしい
        public void PlaceBlock(float3 position, BlockType blockType)
        {
            if (!BlockGridCoordinate.TryParse(position, out var placeCoordinate)) return;
            
            var block = new Block(blockType);
            _chunkBlockSetter.SetBlock(placeCoordinate, block);
        }
    }
}