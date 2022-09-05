using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;

namespace BlockSystem
{
    /// <summary>
    /// 依存関係の解決を行う
    /// </summary>
    public class BlockSystemInitializer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private ChunkObjectStore chunkObjectStore;

        private void Start()
        {
            MasterBlockDataStore.InitialLoad();

            var mapGenerator = new MapGenerator(80);
            var chunkDataStore = new ChunkDataStore(mapGenerator);
            var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
            var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

            new UpdateChunkSystem(player, chunkDataStore, chunkObjectStore, chunkMeshCreator);
        }
    }
}
