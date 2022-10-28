using System;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;

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
        [SerializeField] private BreakBlockSystem breakBlockSystem;

        private IDisposable chunkDataStoreDisposal;

        private void Start()
        {
            MasterBlockDataStore.InitialLoad();

            var mapGenerator = new MapGenerator(1024, 80);
            var chunkDataStore = new ChunkDataStore(mapGenerator);
            chunkDataStoreDisposal = chunkDataStore;
            chunkObjectPool.StartInitial(chunkDataStore);
            var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);
            var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkObjectPool, chunkMeshCreator);

            PlaceBlockSystem.StartInitial(blockDataUpdater);
            breakBlockSystem.StartInitial(blockDataUpdater, chunkDataStore);
            new UpdateChunkSystem(player, chunkObjectPool, chunkDataStore, chunkMeshCreator, this.GetCancellationTokenOnDestroy());
        }

        private void OnApplicationQuit()
        {
            chunkDataStoreDisposal.Dispose();
            ChunkMeshData.DisposeNativeBuffer();
        }
    }
}
