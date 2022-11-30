using System;
using System.Collections.Generic;
using DataStore;
using BlockBehaviour.Interface;

namespace BlockBehaviour
{
    public class BlockBehaviourResolver
    {
        private Dictionary<Type, IBlockBehaviour> instanceLookUpTable = new Dictionary<Type, IBlockBehaviour>();

        public BlockBehaviourResolver(BlockDataAccessor blockDataAccessor)
        {
            instanceLookUpTable.Add(typeof(MUCore), new MUCore(blockDataAccessor));
        }

        public IBlockBehaviour GetBehaviourInstance(Type type)
        {
            return instanceLookUpTable[type];
        }
    }
}
