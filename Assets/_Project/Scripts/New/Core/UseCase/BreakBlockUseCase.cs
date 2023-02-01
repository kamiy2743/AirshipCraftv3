using Domain;
using Domain.Chunks;
using Unity.Mathematics;

namespace UseCase
{
    public class BreakBlockUseCase
    {
        private ChunkBlockSetter chunkBlockSetter;

        internal BreakBlockUseCase(ChunkBlockSetter chunkBlockSetter)
        {
            this.chunkBlockSetter = chunkBlockSetter;
        }

        public void BreakBlock(float3 position)
        {
            if (BlockGridCoordinate.TryParse(position, out var breakCoordinate))
            {
                chunkBlockSetter.SetBlock(breakCoordinate, new Block(BlockTypeID.Air));
            }
        }
    }
}