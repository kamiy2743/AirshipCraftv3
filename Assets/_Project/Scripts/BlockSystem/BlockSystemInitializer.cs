using UnityEngine;
using MasterData.Block;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]
namespace BlockSystem
{
    /// <summary>
    /// 依存関係の解決を行う
    /// </summary>
    internal class BlockSystemInitializer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private ChunkObjectPool chunkObjectPool;

        private void Start()
        {
            MasterBlockDataStore.InitialLoad();

            var mapGenerator = new MapGenerator(1024, 80);
            var chunkDataStore = new ChunkDataStore(mapGenerator);
            var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
            var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);
            chunkObjectPool.StartInitial(chunkDataStore);
            var chunkObjectCreator = new ChunkObjectCreator(chunkObjectPool, chunkDataStore, chunkMeshCreator);
            var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkObjectPool, chunkMeshCreator);

            new PlaceBlockSystem(blockDataUpdater);
            new BreakBlockSystem(blockDataUpdater);
            new UpdateChunkSystem(player, chunkObjectPool, chunkObjectCreator);
        }
    }
}
