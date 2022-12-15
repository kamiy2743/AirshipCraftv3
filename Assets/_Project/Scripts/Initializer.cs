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
    [SerializeField] private Transform chunkObjectParent;
    [SerializeField] private BlockInteractor blockInteractor;

    // TODO prefabはresourcesにおいてもいいかもしれない
    [SerializeField] private ChunkObject chunkObjectPrefab;
    [SerializeField] private DropItem dropItemPrefab;
    [SerializeField] private MUCoreObject muCoreObjectPrefab;

    [SerializeField] private MasterBlockDataSettingsScriptableObject masterBlockDataSettingsScriptableObject;
    [SerializeField] private Material blockMaterial;

    private IDisposable chunkDataFileIODisposal;
    private IDisposable chunkDataStoreDisposal;
    private IDisposable chunkObjectPoolDisposal;
    private IDisposable chunkMeshCreatorDisposal;
    private IDisposable chunkColliderSystemDisposal;
    private IDisposable createChunkAroundPlayerSystemDisposal;
    private IDisposable playerChunkChangeDetectorDisposal;

    private void Start()
    {
        // MasterData
        var masterBlockDataStore = new MasterBlockDataStore(masterBlockDataSettingsScriptableObject);
        new BlockMaterialInitializer(masterBlockDataStore, blockMaterial);

        // DataStore
        var chunkDataFileIO = new ChunkDataFileIO();
        chunkDataFileIODisposal = chunkDataFileIO;
        var chunkDataStore = new ChunkDataStore(chunkDataFileIO);
        chunkDataStoreDisposal = chunkDataStore;
        var chunkObjectPool = new ChunkObjectPool(chunkObjectPrefab, chunkObjectParent);
        chunkObjectPoolDisposal = chunkObjectPool;
        var blockDataAccessor = new BlockDataAccessor(chunkDataStore);

        // ChunkConstruction
        var playerChunkChangeDetector = new PlayerChunkChangeDetector(player);
        playerChunkChangeDetectorDisposal = playerChunkChangeDetector;
        var chunkMeshCreator = new ChunkMeshCreator(masterBlockDataStore, chunkDataStore);
        chunkMeshCreatorDisposal = chunkMeshCreator;
        chunkColliderSystemDisposal = new ChunkColliderSystem(playerChunkChangeDetector, chunkObjectPool);
        createChunkAroundPlayerSystemDisposal = new CreateChunkAroundPlayerSystem(playerChunkChangeDetector, chunkObjectPool, chunkDataStore, chunkMeshCreator);

        // BlockOperator
        var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkDataFileIO, chunkObjectPool, chunkMeshCreator);
        var placeBlockSystem = new PlaceBlockSystem(blockDataUpdater);
        var breakBlockSystem = new BreakBlockSystem(masterBlockDataStore, blockDataUpdater, chunkDataStore, dropItemPrefab);

        // Player
        blockInteractor.StartInitial(masterBlockDataStore, blockDataAccessor, placeBlockSystem, breakBlockSystem);

        // BlockBehaviour
        var blockBehaviourResolver = new BlockBehaviourResolver(blockDataAccessor, blockDataUpdater, muCoreObjectPrefab);

        // BlockBehaviour.Injection
        new BlockBehaviourInjector(blockBehaviourResolver, masterBlockDataStore);
    }

    private void OnApplicationQuit()
    {
        chunkObjectPoolDisposal.Dispose();
        chunkDataFileIODisposal.Dispose();
        chunkDataStoreDisposal.Dispose();
        chunkMeshCreatorDisposal.Dispose();
        ChunkMeshData.DisposeStaticResource();
        playerChunkChangeDetectorDisposal.Dispose();
        chunkColliderSystemDisposal.Dispose();
        createChunkAroundPlayerSystemDisposal.Dispose();
    }
}
