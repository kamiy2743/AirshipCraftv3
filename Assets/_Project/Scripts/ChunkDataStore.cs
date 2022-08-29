using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// 読み込み済みチャンクのデータを管理
    /// 読み込み範囲外のチャンクの保持も想定
    /// </summary>
    public class ChunkDataStore : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        public IReadOnlyDictionary<ChunkCoordinate, ChunkData> LoadedChunks => _loadedChunks;
        private readonly Dictionary<ChunkCoordinate, ChunkData> _loadedChunks = new Dictionary<ChunkCoordinate, ChunkData>();

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// </summary>
        public ChunkData GetChunkData(ChunkCoordinate cc)
        {
            if (_loadedChunks.ContainsKey(cc))
            {
                return _loadedChunks[cc];
            }

            var chunk = new ChunkData(cc);
            _loadedChunks[cc] = chunk;
            return chunk;
        }

        /// <summary>
        /// ChunkDataをもとにChunkObjectを作成します
        /// </summary>
        public ChunkObject CreateChunkObject(ChunkData chunkData)
        {
            var chunkObject = GameObject.Instantiate(chunkObjectPrefab, parent: transform);
            chunkObject.Initial(chunkData);
            return chunkObject;
        }
    }
}
