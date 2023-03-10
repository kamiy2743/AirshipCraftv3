using Domain;

namespace UnityView.ChunkCollision
{
    public class BlockUpdateApplier
    {
        private UpdatedChunkBoundsCalculator updatedChunkBoundsCalculator;
        private ChunkColliderUpdater chunkColliderUpdater;

        internal BlockUpdateApplier(UpdatedChunkBoundsCalculator updatedChunkBoundsCalculator, ChunkColliderUpdater chunkColliderUpdater)
        {
            this.updatedChunkBoundsCalculator = updatedChunkBoundsCalculator;
            this.chunkColliderUpdater = chunkColliderUpdater;
        }

        public void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkBoundsCalculator.Calculate(updateCoordinate);

            foreach (var chunkBounds in results)
            {
                chunkColliderUpdater.Update(chunkBounds);
            }
        }
    }
}