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
    [SerializeField] private Transform chunkRendererParent;
    [SerializeField] private Transform chunkColliderParent;
    [SerializeField] private BlockInteractor blockInteractor;

    // TODO prefabはresourcesにおいてもいいかもしれない
    [SerializeField] private ChunkRenderer chunkRendererPrefab;
    [SerializeField] private ChunkCollider chunkColliderPrefab;
    [SerializeField] private DropItem dropItemPrefab;
    [SerializeField] private MURenderer muRendererPrefab;

    [SerializeField] private MasterBlockDataSettingsScriptableObject masterBlockDataSettingsScriptableObject;
    [SerializeField] private Material blockMaterial;

    private IDisposable chunkDataFileIODisposal;
    private IDisposable chunkDataStoreDisposal;
    private IDisposable chunkRendererPoolDisposal;
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
        var chunkRendererPool = new ChunkRendererPool(chunkRendererPrefab, chunkRendererParent);
        chunkRendererPoolDisposal = chunkRendererPool;
        var blockDataAccessor = new BlockDataAccessor(chunkDataStore);

        // ChunkConstruction
        var playerChunkChangeDetector = new PlayerChunkChangeDetector(player);
        playerChunkChangeDetectorDisposal = playerChunkChangeDetector;
        var chunkMeshCreatorUtil = new ChunkMeshCreatorUtil(chunkDataStore);
        var meshCombiner = new MeshCombiner(masterBlockDataStore);
        var chunkMeshCreator = new ChunkMeshCreator(chunkMeshCreatorUtil, meshCombiner);
        chunkMeshCreatorDisposal = chunkMeshCreator;
        chunkColliderSystemDisposal = new ChunkColliderSystem(playerChunkChangeDetector, chunkDataStore, chunkMeshCreatorUtil, chunkColliderPrefab, chunkColliderParent);
        createChunkAroundPlayerSystemDisposal = new CreateChunkAroundPlayerSystem(playerChunkChangeDetector, chunkRendererPool, chunkDataStore, chunkMeshCreator);

        // BlockOperator
        var blockDataUpdater = new BlockDataUpdater(chunkDataStore, chunkDataFileIO, chunkRendererPool, chunkMeshCreator);
        var placeBlockSystem = new PlaceBlockSystem(blockDataUpdater);
        var breakBlockSystem = new BreakBlockSystem(masterBlockDataStore, blockDataUpdater, chunkDataStore, dropItemPrefab);

        // Player
        blockInteractor.StartInitial(masterBlockDataStore, blockDataAccessor, placeBlockSystem, breakBlockSystem);

        // BlockBehaviour
        var blockBehaviourResolver = new BlockBehaviourResolver(blockDataAccessor, blockDataUpdater, meshCombiner, muRendererPrefab);

        // BlockBehaviour.Injection
        new BlockBehaviourInjector(blockBehaviourResolver, masterBlockDataStore);
    }

    private void OnApplicationQuit()
    {
        chunkRendererPoolDisposal.Dispose();
        chunkDataFileIODisposal.Dispose();
        chunkDataStoreDisposal.Dispose();
        chunkMeshCreatorDisposal.Dispose();
        playerChunkChangeDetectorDisposal.Dispose();
        chunkColliderSystemDisposal.Dispose();
        createChunkAroundPlayerSystemDisposal.Dispose();
    }
}
