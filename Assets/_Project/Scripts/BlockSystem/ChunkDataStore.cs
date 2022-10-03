using System.IO;
using System.Threading;
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
        /// キャンセルされた場合はnullを返す
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            if (chunks.ContainsKey(cc))
            {
                return (ChunkData)chunks[cc];
            }

            if (ct.IsCancellationRequested) return null;

            lock (chunks)
            {
                ChunkData chunkData = null;

                if (chunkDataIndexDictionary.ContainsKey(cc))
                {
                    chunkData = ReadChunk((int)chunkDataIndexDictionary[cc], ct);
                }
                else
                {
                    chunkData = CreateNewChunk(cc, ct);
                }

                if (chunkData == null) return null;

                chunks.Add(cc, chunkData);
                return chunkData;
            }
        }

        private ChunkData CreateNewChunk(ChunkCoordinate cc, CancellationToken ct)
        {
            var newChunkData = new ChunkData(cc, _mapGenerator);

            chunkDataIndexDictionary.Add(cc, chunkDataIndexDictionary.Count);

            try
            {
                using (var fs = new FileStream(IndexDictionaryPath, FileMode.Create, FileAccess.Write))
                {
                    MessagePackSerializer.Serialize(fs, chunkDataIndexDictionary, cancellationToken: ct);
                }

                using (var fs = new FileStream(DataFilePath, FileMode.Append, FileAccess.Write))
                {
                    MessagePackSerializer.Serialize(fs, newChunkData, cancellationToken: ct);
                }
            }
            catch (MessagePackSerializationException)
            {
                chunkDataIndexDictionary.Remove(cc);
                return null;
            }
            catch (System.OperationCanceledException)
            {
                chunkDataIndexDictionary.Remove(cc);
                return null;
            }

            return newChunkData;
        }

        private ChunkData ReadChunk(int index, CancellationToken ct)
        {
            try
            {
                using (var fs = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Position = ChunkDataByteSize * index;
                    using (var br = new BinaryReader(fs))
                    {
                        var bytes = br.ReadBytes(ChunkDataByteSize);
                        return MessagePackSerializer.Deserialize<ChunkData>(bytes, cancellationToken: ct);
                    }
                }
            }
            catch (MessagePackSerializationException)
            {
                return null;
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }
        }
    }
}
