using System.Collections;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータを管理
    /// </summary>
    internal class ChunkDataStore
    {
        private readonly Hashtable chunks = new Hashtable();
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
            if (chunks.ContainsKey(cc))
            {
                return (ChunkData)chunks[cc];
            }

            lock (chunks)
            {
                var newChunkData = new ChunkData(cc, _mapGenerator);
                chunks.Add(cc, newChunkData);
                return newChunkData;
            }
        }
    }
}
