using DataObject.Block;

namespace MasterData.Block
{
    public class MUCore : IInteractedBehaviour
    {
        public void OnInteracted(BlockData targetBlockData)
        {
            UnityEngine.Debug.Log("MUCore");
            UnityEngine.Debug.Log(targetBlockData.BlockCoordinate);
        }
    }
}
