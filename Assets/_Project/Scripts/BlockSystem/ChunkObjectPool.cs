using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクオブジェクトを管理
    /// </summary>
    public class ChunkObjectPool : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        public IReadOnlyDictionary<ChunkCoordinate, ChunkObject> ChunkObjects => _chunkObjects;
        private ConcurrentDictionary<ChunkCoordinate, ChunkObject> _chunkObjects = new ConcurrentDictionary<ChunkCoordinate, ChunkObject>();
        private ConcurrentQueue<ChunkObject> availableChunkObjects = new ConcurrentQueue<ChunkObject>();

        internal void StartInitial()
        {
            for (int i = 0; i < World.LoadChunkCount; i++)
            {
                var chunkObject = Instantiate(chunkObjectPrefab, parent: transform);
                chunkObject.gameObject.SetActive(false);
                availableChunkObjects.Enqueue(chunkObject);
            }
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        public ChunkObject GetChunkObject(ChunkCoordinate cc)
        {
            if (availableChunkObjects.Count == 0)
            {
                throw new System.Exception("ChunkObjectプールが空です");
            }

            if (!availableChunkObjects.TryDequeue(out ChunkObject chunkObject))
            {
                throw new System.Exception("failed");
            }

            if (!_chunkObjects.TryAdd(cc, chunkObject))
            {
                throw new System.Exception("failed");
            }

            chunkObject.gameObject.SetActive(true);

            return chunkObject;
        }

        /// <summary>
        /// オブジェクトプールに投げる
        /// メインスレッドのみ
        /// </summary>
        public void ReleaseChunkObject(ChunkCoordinate cc)
        {
            if (!_chunkObjects.TryRemove(cc, out ChunkObject chunkObject))
            {
                throw new System.Exception("failed");
            }

            chunkObject.gameObject.SetActive(false);
            availableChunkObjects.Enqueue(chunkObject);
        }
    }
}