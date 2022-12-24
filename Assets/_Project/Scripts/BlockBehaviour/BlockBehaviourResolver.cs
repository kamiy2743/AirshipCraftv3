using System;
using System.Collections.Generic;
using DataStore;
using BlockBehaviour.Interface;
using BlockOperator;

namespace BlockBehaviour
{
    public class BlockBehaviourResolver
    {
        private Dictionary<Type, IBlockBehaviour> instanceLookUpTable = new Dictionary<Type, IBlockBehaviour>();

        public BlockBehaviourResolver(BlockDataAccessor blockDataAccessor, BlockDataUpdater blockDataUpdater, MUCoreRenderer muCoreRendererPrefab)
        {
            instanceLookUpTable.Add(typeof(MUCore), new MUCore(blockDataAccessor, blockDataUpdater, muCoreRendererPrefab));
        }

        public IBlockBehaviour GetBehaviourInstance(Type type)
        {
            return instanceLookUpTable[type];
        }
    }
}
