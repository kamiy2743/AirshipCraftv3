using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクオブジェクトを管理
    /// </summary>
    public class ChunkObjectStore : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        private ChunkMeshCreator _chunkMeshCreator;

        internal void StartInitial(ChunkMeshCreator chunkMeshCreator)
        {
            _chunkMeshCreator = chunkMeshCreator;
        }

        /// <summary>
        /// ChunkDataをもとにChunkObjectを作成します
        /// </summary>
        public ChunkObject CreateChunkObject(ChunkData chunkData)
        {
            var chunkObject = GameObject.Instantiate(chunkObjectPrefab, parent: transform);
            chunkObject.SetMesh(_chunkMeshCreator.CreateMesh(chunkData));
            return chunkObject;
        }
    }
}
