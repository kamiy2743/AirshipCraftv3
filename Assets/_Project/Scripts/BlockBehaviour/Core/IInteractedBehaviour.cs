using DataObject.Block;

namespace BlockBehaviour.Core
{
    public interface IInteractedBehaviour
    {
        void OnInteracted(BlockData targetBlockData);
    }
}
