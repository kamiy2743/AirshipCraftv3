using System.Collections.Generic;
using DataObject.Block;
using BlockBehaviour.Interface;
using DataStore;

namespace BlockBehaviour
{
    internal class MUCore : IBlockBehaviour, IInteractedBehaviour
    {
        private BlockDataAccessor _blockDataAccessor;

        internal MUCore(BlockDataAccessor blockDataAccessor)
        {
            _blockDataAccessor = blockDataAccessor;
        }

        public void OnInteracted(BlockData targetBlockData)
        {

        }
    }
}
