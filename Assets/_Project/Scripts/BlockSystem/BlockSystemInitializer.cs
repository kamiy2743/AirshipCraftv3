using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var chunkDataStore = new ChunkDataStore();
            var contactOtherBlockSolver = new ContactOtherBlockSolver(chunkDataStore);
            var chunkMeshCreator = new ChunkMeshCreator(contactOtherBlockSolver);

            chunkObjectStore.StartInitial(chunkMeshCreator);

            new UpdateChunkSystem(player, chunkDataStore, chunkObjectStore);
        }
    }
}
