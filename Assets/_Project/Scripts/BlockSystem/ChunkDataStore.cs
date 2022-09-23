using System.Collections.Generic;
using System.Collections.Concurrent;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータを管理
    /// </summary>
    internal class ChunkDataStore
    {
        internal IReadOnlyDictionary<ChunkCoordinate, ChunkData> Chunks => _chunks;
        private readonly ConcurrentDictionary<ChunkCoordinate, ChunkData> _chunks = new ConcurrentDictionary<ChunkCoordinate, ChunkData>();

        private MapGenerator _mapGenerator;

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc)
        {
            return _chunks.GetOrAdd(cc, _ => new ChunkData(cc, _mapGenerator));
        }
    }
}
