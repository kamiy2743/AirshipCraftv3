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

        /// <summary>
        /// ChunkObjectを作成します
        /// メインスレッドのみ
        /// </summary>
        public ChunkObject CreateChunkObject(Mesh mesh)
        {
            var chunkObject = GameObject.Instantiate(chunkObjectPrefab, parent: transform);
            chunkObject.SetMesh(mesh);
            return chunkObject;
        }
    }
}
