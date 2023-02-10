using Domain;

namespace UnityView.ChunkCollision
{
    public class BlockUpdateApplier
    {
        readonly UpdatedChunkBoundsCalculator _updatedChunkBoundsCalculator;
        readonly ChunkColliderUpdater _chunkColliderUpdater;

        internal BlockUpdateApplier(UpdatedChunkBoundsCalculator updatedChunkBoundsCalculator, ChunkColliderUpdater chunkColliderUpdater)
        {
            _updatedChunkBoundsCalculator = updatedChunkBoundsCalculator;
            _chunkColliderUpdater = chunkColliderUpdater;
        }

        public void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = _updatedChunkBoundsCalculator.Calculate(updateCoordinate);

            foreach (var chunkBounds in results)
            {
                _chunkColliderUpdater.Update(chunkBounds);
            }
        }
    }
}