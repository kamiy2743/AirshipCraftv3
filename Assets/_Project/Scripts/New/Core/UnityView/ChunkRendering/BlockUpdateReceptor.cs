using Domain;

namespace UnityView.ChunkRendering
{
    public class BlockUpdateReceptor
    {
        private BlockUpdateApplier blockUpdateApplier;

        internal BlockUpdateReceptor(BlockUpdateApplier blockUpdateApplier)
        {
            this.blockUpdateApplier = blockUpdateApplier;
        }

        public void UpdateBlock(BlockGridCoordinate updateCoordinate)
        {
            blockUpdateApplier.Apply(updateCoordinate);
        }
    }
}