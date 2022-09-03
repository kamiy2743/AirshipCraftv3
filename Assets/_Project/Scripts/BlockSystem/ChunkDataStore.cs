using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータを管理
    /// </summary>
    public class ChunkDataStore
    {
        public IReadOnlyDictionary<ChunkCoordinate, ChunkData> Chunks => _chunks;
        private readonly Dictionary<ChunkCoordinate, ChunkData> _chunks = new Dictionary<ChunkCoordinate, ChunkData>();

        private MapGenerator _mapGenerator;

        public ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// </summary>
        public ChunkData GetChunkData(ChunkCoordinate cc)
        {
            if (_chunks.ContainsKey(cc))
            {
                return _chunks[cc];
            }

            var chunk = new ChunkData(cc, _mapGenerator);
            _chunks[cc] = chunk;
            return chunk;
        }
    }
}
