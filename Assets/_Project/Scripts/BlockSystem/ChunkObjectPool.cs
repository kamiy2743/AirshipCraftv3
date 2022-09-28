using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクオブジェクトを管理
    /// </summary>
    internal class ChunkObjectPool : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        internal IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects => _chunkObjects;
        private Dictionary<ChunkCoordinate, ChunkObject> _chunkObjects;

        internal IReadOnlyCollection<ChunkCoordinate> CreatedChunkHashSet => _createdChunkHashSet;
        private HashSet<ChunkCoordinate> _createdChunkHashSet;

        private Queue<ChunkObject> availableChunkObjects;

        internal void StartInitial(ChunkDataStore chunkDataStore)
        {
            var capacity = World.LoadChunkCount;
            _chunkObjects = new Dictionary<ChunkCoordinate, ChunkObject>(capacity);
            _createdChunkHashSet = new HashSet<ChunkCoordinate>(capacity);
            availableChunkObjects = new Queue<ChunkObject>(capacity);

            for (int i = 0; i < World.LoadChunkCount; i++)
            {
                var chunkObject = Instantiate(chunkObjectPrefab, parent: transform);
                chunkObject.Init(chunkDataStore);
                availableChunkObjects.Enqueue(chunkObject);
            }
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        internal ChunkObject TakeChunkObject(ChunkCoordinate cc)
        {
            if (availableChunkObjects.Count == 0)
            {
                throw new System.Exception("ChunkObjectプールが空です");
            }

            var chunkObject = availableChunkObjects.Dequeue();
            _chunkObjects.Add(cc, chunkObject);
            _createdChunkHashSet.Add(cc);

            return chunkObject;
        }

        /// <summary>
        /// オブジェクトプールに投げる
        /// メインスレッドのみ
        /// </summary>
        internal void ReleaseChunkObject(ChunkCoordinate cc)
        {
            if (!_chunkObjects.TryGetValue(cc, out var chunkObject))
            {
                throw new System.Exception("割り当てられたChunkObjectが存在しません: " + cc);
            }

            chunkObject.ClearMesh();
            _chunkObjects.Remove(cc);
            _createdChunkHashSet.Remove(cc);
            availableChunkObjects.Enqueue(chunkObject);
        }
    }
}
