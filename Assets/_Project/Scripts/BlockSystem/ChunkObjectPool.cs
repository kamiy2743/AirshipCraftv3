using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BlockSystem
{
    /// <summary>
    /// チャンクオブジェクトを管理
    /// </summary>
    internal class ChunkObjectPool : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        private ChunkDataStore _chunkDataStore;
        private static readonly int Capacity = World.LoadChunkCount;

        internal IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects => _chunkObjects;
        private Dictionary<ChunkCoordinate, ChunkObject> _chunkObjects = new Dictionary<ChunkCoordinate, ChunkObject>(Capacity);
        internal HashSet<ChunkCoordinate> CreatedChunkHashSet => _chunkObjects.Keys.ToHashSet();

        private Queue<ChunkObject> availableChunkObjectQueue = new Queue<ChunkObject>(Capacity);

        internal void StartInitial(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        internal ChunkObject TakeChunkObject(ChunkCoordinate cc)
        {
            if (_chunkObjects.Count + 1 >= Capacity)
            {
                throw new System.Exception("Capacity: " + Capacity + " を超えたのでこれ以上取り出せません");
            }

            ChunkObject chunkObject;
            if (availableChunkObjectQueue.Count > 0)
            {
                chunkObject = availableChunkObjectQueue.Dequeue();
            }
            else
            {
                chunkObject = Instantiate(chunkObjectPrefab, parent: transform);
                chunkObject.Init(_chunkDataStore);
            }

            _chunkObjects.Add(cc, chunkObject);
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
            availableChunkObjectQueue.Enqueue(chunkObject);
        }
    }
}
