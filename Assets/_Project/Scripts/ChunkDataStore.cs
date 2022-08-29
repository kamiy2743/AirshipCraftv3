using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータとオブジェクトを管理
    /// </summary>
    public class ChunkDataStore : MonoBehaviour
    {
        [SerializeField] private ChunkObject chunkObjectPrefab;

        public IReadOnlyDictionary<ChunkCoordinate, ChunkData> Chunks => _chunks;
        private readonly Dictionary<ChunkCoordinate, ChunkData> _chunks = new Dictionary<ChunkCoordinate, ChunkData>();

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// </summary>
        public ChunkData GetChunkData(ChunkCoordinate cc)
        {
            if (_chunks.ContainsKey(cc))
            {
                return _chunks[cc];
            }

            var chunk = new ChunkData(cc);
            _chunks[cc] = chunk;
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
