using System;
using UnityEngine;
using MasterData.Block;
using DataStore;
using BlockOperator;
using ChunkConstruction;
using DataObject.Chunk;
using Player;
using BlockBehaviour;
using BlockBehaviour.Injection;

/// <summary>
/// 依存関係の解決を行う 
/// </summary>
internal class Initializer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ChunkObjectPool chunkObjectPool;
    [SerializeField] private BreakBlockSystem breakBlockSystem;
    [SerializeField] private BlockInteractor blockInteractor;

    // TODO prefabはresourcesにおいてもいいかもしれない
    [SerializeField] private DropItem dropItemPrefab;
    [SerializeField] private MUCoreObject muCoreObjectPrefab;

    private IDisposable chunkDataFileIODisposal;
    private IDisposable chunkDataStoreDisposal;
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
        chunkDataStoreDisposal = chunkDataStore;
        var blockDataAccessor = new BlockDataAccessor(chunkDataStore);
        chunkObjectPool.StartInitial();
        var chunkMeshCreator = new ChunkMeshCreator(chunkDataStore);
        chunkMeshCreatorDisposal = chunkMeshCreator;
        var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkDataFileIO, chunkObjectPool, chunkMeshCreator);
        var blockBehaviourResolver = new BlockBehaviourResolver(blockDataAccessor, blockDataUpdater, muCoreObjectPrefab);
        new BlockBehaviourInjector(blockBehaviourResolver);
        var playerChunkChangeDetector = new PlayerChunkChangeDetector(player);
        playerChunkChangeDetectorDisposal = playerChunkChangeDetector;

        var placeBlockSystem = new PlaceBlockSystem(blockDataUpdater);
        var breakBlockStream = new BreakBlockSystem(blockDataUpdater, chunkDataStore, dropItemPrefab);
        blockInteractor.StartInitial(blockDataAccessor, placeBlockSystem, breakBlockSystem);
        chunkColliderSystemDisposal = new ChunkColliderSystem(playerChunkChangeDetector, chunkObjectPool);
        createChunkAroundPlayerSystemDisposal = new CreateChunkAroundPlayerSystem(playerChunkChangeDetector, chunkObjectPool, chunkDataStore, chunkMeshCreator);
    }

    private void OnApplicationQuit()
    {
        chunkObjectPool.Dispose();
        chunkDataFileIODisposal.Dispose();
        chunkDataStoreDisposal.Dispose();
        chunkMeshCreatorDisposal.Dispose();
        ChunkMeshData.DisposeStaticResource();
        playerChunkChangeDetectorDisposal.Dispose();
        chunkColliderSystemDisposal.Dispose();
        createChunkAroundPlayerSystemDisposal.Dispose();
    }
}
