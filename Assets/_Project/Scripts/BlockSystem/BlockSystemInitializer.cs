using System;
using UnityEngine;
using MasterData.Block;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]
namespace BlockSystem
{
    /// <summary> 依存関係の解決を行う </summary>
    internal class BlockSystemInitializer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private ChunkObjectPool chunkObjectPool;
        [SerializeField] private BreakBlockSystem breakBlockSystem;

        private IDisposable chunkDataFileIODisposal;
        private IDisposable chunkMeshCreatorDisposal;
        private IDisposable chunkColliderSystemDisposal;
        private IDisposable createChunkAroundPlayerSystemDisposal;
        private IDisposable playerChunkChangeDetectorDisposal;

        private void Start()
        {
            MasterBlockDataStore.InitialLoad();

            var chunkDataFileIO = new ChunkDataFileIO();
            chunkDataFileIODisposal = chunkDataFileIO;
            var chunkDataStore = new ChunkDataStore(chunkDataFileIO);
            chunkObjectPool.StartInitial(chunkDataStore);
            var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);
            chunkMeshCreatorDisposal = chunkMeshCreator;
            var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkDataFileIO, chunkObjectPool, chunkMeshCreator);
            var playerChunkChangeDetector = new PlayerChunkChangeDetector(player);
            playerChunkChangeDetectorDisposal = playerChunkChangeDetector;

            PlaceBlockSystem.StartInitial(blockDataUpdater);
            breakBlockSystem.StartInitial(blockDataUpdater, chunkDataStore);
            chunkColliderSystemDisposal = new ChunkColliderSystem(playerChunkChangeDetector, chunkObjectPool);
            createChunkAroundPlayerSystemDisposal = new CreateChunkAroundPlayerSystem(playerChunkChangeDetector, chunkObjectPool, chunkDataStore, chunkMeshCreator);
        }

        private void OnApplicationQuit()
        {
            chunkObjectPool.Dispose();
            chunkDataFileIODisposal.Dispose();
            chunkMeshCreatorDisposal.Dispose();
            ChunkMeshData.DisposeNativeBuffer();
            playerChunkChangeDetectorDisposal.Dispose();
            chunkColliderSystemDisposal.Dispose();
            createChunkAroundPlayerSystemDisposal.Dispose();
        }
    }
}
