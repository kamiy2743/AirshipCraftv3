using System;
using System.Collections.Generic;
using DataStore;
using ChunkConstruction;
using BlockBehaviour.Interface;
using BlockOperator;

namespace BlockBehaviour
{
    public class BlockBehaviourResolver
    {
        private Dictionary<Type, IBlockBehaviour> instanceLookUpTable = new Dictionary<Type, IBlockBehaviour>();

        public BlockBehaviourResolver(BlockDataAccessor blockDataAccessor, BlockDataUpdater blockDataUpdater, MeshCombiner meshCombiner, MURenderer muRendererPrefab, MUCollider muColliderPrefab)
        {
            instanceLookUpTable.Add(typeof(MUCore), new MUCore(blockDataAccessor, blockDataUpdater, meshCombiner, muRendererPrefab, muColliderPrefab));
        }

        public IBlockBehaviour GetBehaviourInstance(Type type)
        {
            return instanceLookUpTable[type];
        }
    }
}
