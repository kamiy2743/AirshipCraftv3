using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    public class BreakBlockUseCase
    {
        readonly ChunkBlockSetter chunkBlockSetter;

        internal BreakBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            this.chunkBlockSetter = chunkBlockSetter;
        }

        public void BreakBlock(float3 position)
        {
            if (BlockGridCoordinate.TryParse(position, out var breakCoordinate))
            {
                chunkBlockSetter.SetBlock(breakCoordinate, new Block(BlockType.Air));
            }
        }
    }
}