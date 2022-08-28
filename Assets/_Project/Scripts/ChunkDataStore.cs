using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// 読み込み済みチャンクのデータを管理
    /// 読み込み範囲外のチャンクの保持も想定
    /// </summary>
    public class ChunkDataStore
    {
        public IReadOnlyDictionary<ChunkCoordinate, ChunkData> LoadedChunks => _loadedChunks;
        private readonly Dictionary<ChunkCoordinate, ChunkData> _loadedChunks = new Dictionary<ChunkCoordinate, ChunkData>();
    }
}
