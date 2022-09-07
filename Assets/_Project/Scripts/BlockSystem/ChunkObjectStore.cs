using System.Collections.Concurrent;
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

        public IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects => _chunkObjects;
        private Dictionary<ChunkCoordinate, ChunkObject> _chunkObjects = new Dictionary<ChunkCoordinate, ChunkObject>();

        /// <summary>
        /// ChunkObjectを作成します
        /// メインスレッドのみ
        /// </summary>
        public ChunkObject CreateChunkObject(ChunkCoordinate cc, Mesh mesh)
        {
            var chunkObject = GameObject.Instantiate(chunkObjectPrefab, parent: transform);
            chunkObject.SetMesh(mesh);
            _chunkObjects.Add(cc, chunkObject);

            return chunkObject;
        }

        public void DisposeChunkObject(ChunkCoordinate cc)
        {
            Destroy(_chunkObjects[cc].gameObject);
            _chunkObjects.Remove(cc);
        }
    }
}
