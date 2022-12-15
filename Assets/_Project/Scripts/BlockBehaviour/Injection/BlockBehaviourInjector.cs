using System;
using System.Reflection;
using MasterData.Block;

namespace BlockBehaviour.Injection
{
    public class BlockBehaviourInjector
    {
        public BlockBehaviourInjector(BlockBehaviourResolver blockBehaviourResolver, MasterBlockDataStore masterBlockDataStore)
        {
            var nameSpace = nameof(BlockBehaviour);
            var assembly = Assembly.GetAssembly(typeof(BlockBehaviourResolver));

            foreach (var masterBlockData in masterBlockDataStore.MasterBlockDataList)
            {
                var behaviourType = Type.GetType($"{nameSpace}.{masterBlockData.Name}, {assembly}");
                if (behaviourType is null) continue;

                var behaviour = blockBehaviourResolver.GetBehaviourInstance(behaviourType);
                masterBlockData.SetBlockBehaviour(behaviour);
            }
        }
    }
}
