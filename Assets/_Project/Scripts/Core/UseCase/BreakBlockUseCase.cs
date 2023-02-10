using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    public class BreakBlockUseCase
    {
        readonly ChunkBlockSetter _chunkBlockSetter;

        internal BreakBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            _chunkBlockSetter = chunkBlockSetter;
        }

        public void BreakBlock(float3 position)
        {
            if (BlockGridCoordinate.TryParse(position, out var breakCoordinate))
            {
                _chunkBlockSetter.SetBlock(breakCoordinate, new Block(BlockType.Air));
            }
        }
    }
}