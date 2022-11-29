using DataObject.Block;

namespace MasterData.Block
{
    public interface IInteractedBehaviour
    {
        void OnInteracted(BlockData targetBlockData);
    }
}
