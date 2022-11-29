using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using DataObject.Chunk;

namespace DataStore
{
    /// <summary> チャンクオブジェクトを管理 </summary>
    public class ChunkObjectPool : MonoBehaviour, IDisposable
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        private static readonly int Capacity = World.LoadChunkCount;

        public NativeParallelHashSet<ChunkCoordinate> CreatedChunks;
        public IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects => _chunkObjects;
        private Dictionary<ChunkCoordinate, ChunkObject> _chunkObjects = new Dictionary<ChunkCoordinate, ChunkObject>(Capacity);

        private Queue<ChunkObject> availableChunkObjectQueue = new Queue<ChunkObject>(Capacity);

        public void StartInitial()
        {
            CreatedChunks = new NativeParallelHashSet<ChunkCoordinate>(Capacity, Allocator.Persistent);
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        public ChunkObject TakeChunkObject(ChunkCoordinate cc)
        {
            if (_chunkObjects.Count + 1 > Capacity)
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
            }

            CreatedChunks.Add(cc);
            _chunkObjects.Add(cc, chunkObject);

            return chunkObject;
        }

        /// <summary>
        /// オブジェクトプールに投げる
        /// メインスレッドのみ
        /// </summary>
        public void ReleaseChunkObject(ChunkCoordinate cc)
        {
            if (!_chunkObjects.TryGetValue(cc, out var chunkObject))
            {
                throw new System.Exception("割り当てられたChunkObjectが存在しません: " + cc);
            }

            chunkObject.ClearMesh();
            CreatedChunks.Remove(cc);
            _chunkObjects.Remove(cc);
            availableChunkObjectQueue.Enqueue(chunkObject);
        }

        public void Dispose()
        {
            CreatedChunks.Dispose();
        }
    }
}
