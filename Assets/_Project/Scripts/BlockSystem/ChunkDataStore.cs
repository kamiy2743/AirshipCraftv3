using System.IO;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータを管理
    /// </summary>
    internal class ChunkDataStore
    {
        private Hashtable chunks = new Hashtable();
        private Dictionary<ChunkCoordinate, int> chunkDataIndexDictionary = new Dictionary<ChunkCoordinate, int>();
        private MapGenerator _mapGenerator;

        private static int ChunkDataByteSize;
        private static readonly string DataFilePath = Application.persistentDataPath + "/ChunkDataStore/ChunkData.bin";
        private static readonly string IndexDictionaryPath = Application.persistentDataPath + "/ChunkDataStore/IndexDictionary.bin";

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;

            ChunkDataByteSize = MessagePackSerializer.Serialize(ChunkData.Empty).Length;

            if (File.Exists(IndexDictionaryPath))
            {
                using (var fs = new FileStream(IndexDictionaryPath, FileMode.Open, FileAccess.Read))
                {
                    chunkDataIndexDictionary = MessagePackSerializer.Deserialize<Dictionary<ChunkCoordinate, int>>(fs);
                }
            }
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
                ChunkData chunkData = null;

                if (chunkDataIndexDictionary.ContainsKey(cc))
                {
                    chunkData = ReadChunk((int)chunkDataIndexDictionary[cc]);
                }
                else
                {
                    chunkData = CreateNewChunk(cc);
                }

                if (chunkData == null) return null;

                chunks.Add(cc, chunkData);
                return chunkData;
            }
        }

        private ChunkData CreateNewChunk(ChunkCoordinate cc)
        {
            var newChunkData = new ChunkData(cc, _mapGenerator);

            chunkDataIndexDictionary.Add(cc, chunkDataIndexDictionary.Count);
            using (var fs = new FileStream(IndexDictionaryPath, FileMode.Create, FileAccess.Write))
            {
                MessagePackSerializer.Serialize(fs, chunkDataIndexDictionary);
            }

            using (var fs = new FileStream(DataFilePath, FileMode.Append, FileAccess.Write))
            {
                MessagePackSerializer.Serialize<ChunkData>(fs, newChunkData);
            }

            return newChunkData;
        }

        private ChunkData ReadChunk(int index)
        {
            using (var fs = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read))
            {
                fs.Position = ChunkDataByteSize * index;
                return MessagePackSerializer.Deserialize<ChunkData>(fs);
            }
        }
    }
}
