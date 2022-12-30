using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;
using DataObject.Chunk;

namespace DataStore
{
    /// <summary> 
    /// チャンクオブジェクトを管理 
    /// </summary>
    public class ChunkRendererPool : IDisposable
    {
        private ChunkRenderer _chunkRendererPrefab;
        private Transform _chunkRendererParent;

        private static readonly int Capacity = World.LoadChunkCount;

        public NativeParallelHashSet<ChunkCoordinate> CreatedChunks = new NativeParallelHashSet<ChunkCoordinate>(Capacity, Allocator.Persistent);
        public IReadOnlyDictionary<ChunkCoordinate, ChunkRenderer> ChunkRenderers => _chunkRenderers;
        private Dictionary<ChunkCoordinate, ChunkRenderer> _chunkRenderers = new Dictionary<ChunkCoordinate, ChunkRenderer>(Capacity);

        private Queue<ChunkRenderer> availableChunkRendererQueue = new Queue<ChunkRenderer>(Capacity);

        public ChunkRendererPool(ChunkRenderer chunkRendererPrefab, Transform chunkRendererParent)
        {
            _chunkRendererPrefab = chunkRendererPrefab;
            _chunkRendererParent = chunkRendererParent;
        }

        /// <summary>
        /// オブジェクトプールから取得
        /// メインスレッドのみ
        /// </summary>
        public ChunkRenderer TakeChunkRenderer(ChunkCoordinate cc)
        {
            if (_chunkRenderers.Count + 1 > Capacity)
            {
                throw new System.Exception("Capacity: " + Capacity + " を超えたのでこれ以上取り出せません");
            }

            ChunkRenderer chunkRenderer;
            if (availableChunkRendererQueue.Count > 0)
            {
                chunkRenderer = availableChunkRendererQueue.Dequeue();
            }
            else
            {
                chunkRenderer = MonoBehaviour.Instantiate(_chunkRendererPrefab, parent: _chunkRendererParent);
            }

            CreatedChunks.Add(cc);
            _chunkRenderers.Add(cc, chunkRenderer);

            return chunkRenderer;
        }

        /// <summary>
        /// オブジェクトプールに投げる
        /// メインスレッドのみ
        /// </summary>
        public void ReleaseChunkRenderer(ChunkCoordinate cc)
        {
            if (!_chunkRenderers.TryGetValue(cc, out var chunkRenderer))
            {
                throw new System.Exception("割り当てられたChunkRendererが存在しません: " + cc);
            }

            chunkRenderer.ClearMesh();
            CreatedChunks.Remove(cc);
            _chunkRenderers.Remove(cc);
            availableChunkRendererQueue.Enqueue(chunkRenderer);
        }

        public void Dispose()
        {
            CreatedChunks.Dispose();
        }
    }
}
