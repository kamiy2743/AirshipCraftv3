using System.Collections.Concurrent;
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
        private ConcurrentDictionary<ChunkCoordinate, ChunkObject> _chunkObjects = new ConcurrentDictionary<ChunkCoordinate, ChunkObject>();
        private ConcurrentBag<ChunkObject> availableChunkObjects = new ConcurrentBag<ChunkObject>();

        internal void StartInitial(ChunkDataStore chunkDataStore)
        {
            for (int i = 0; i < World.LoadChunkCount; i++)
            {
                var chunkObject = Instantiate(chunkObjectPrefab, parent: transform);
                chunkObject.Init(chunkDataStore);
                chunkObject.gameObject.SetActive(false);
                availableChunkObjects.Add(chunkObject);
            }
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        internal ChunkObject GetChunkObject(ChunkCoordinate cc)
        {
            if (availableChunkObjects.Count == 0)
            {
                throw new System.Exception("ChunkObjectプールが空です");
            }

            if (!availableChunkObjects.TryTake(out ChunkObject chunkObject))
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
        internal void ReleaseChunkObject(ChunkCoordinate cc)
        {
            if (!_chunkObjects.TryRemove(cc, out ChunkObject chunkObject))
            {
                throw new System.Exception("failed");
            }

            chunkObject.gameObject.SetActive(false);
            availableChunkObjects.Add(chunkObject);
        }
    }
}
