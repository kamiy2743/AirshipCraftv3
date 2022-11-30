using DataObject.Block;

namespace BlockBehaviour.Interface
{
    public interface IInteractedBehaviour
    {
        void OnInteracted(BlockData targetBlockData);
    }
}
