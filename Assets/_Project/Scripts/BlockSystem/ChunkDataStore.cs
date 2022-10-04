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
        private Hashtable chunkDataIndexHashtable = new Hashtable();
        private MapGenerator _mapGenerator;

        private static int ChunkDataByteSize;
        private static readonly string DataFilePath = Application.persistentDataPath + "/ChunkDataStore/ChunkData.bin";
        private static readonly string IndexHashtablePath = Application.persistentDataPath + "/ChunkDataStore/IndexHashtable.bin";

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;

            ChunkDataByteSize = MessagePackSerializer.Serialize(ChunkData.Empty).Length;
            var chunkDataIndexByteSize = MessagePackSerializer.Serialize(new ChunkDataIndex()).Length;

            if (File.Exists(IndexHashtablePath))
            {
                using (var fs = new FileStream(IndexHashtablePath, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        var bytes = br.ReadBytes(chunkDataIndexByteSize);
                        var chunkDataIndex = MessagePackSerializer.Deserialize<ChunkDataIndex>(bytes);
                        chunkDataIndexHashtable.Add(chunkDataIndex.ChunkCoordinate, chunkDataIndex.Index);
                    }
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

                if (chunkDataIndexHashtable.ContainsKey(cc))
                {
                    chunkData = ReadChunk((long)chunkDataIndexHashtable[cc]);
                    if (chunkData == null) return null;
                }
                else
                {
                    chunkData = CreateNewChunk(cc);
                }

                chunks.Add(cc, chunkData);
                if (ct.IsCancellationRequested) return null;

                return chunkData;
            }
        }

        private ChunkData CreateNewChunk(ChunkCoordinate cc)
        {
            var newChunkData = new ChunkData(cc, _mapGenerator);

            // TODO 事実上intの最大値までしか正常に動かない
            var chunkDataIndex = (long)chunkDataIndexHashtable.Count;
            chunkDataIndexHashtable.Add(cc, chunkDataIndex);

            // TODO FileStreamのコンストラクタがかなり遅い
            using (var fs = new FileStream(IndexHashtablePath, FileMode.Append, FileAccess.Write))
            {
                MessagePackSerializer.Serialize(fs, new ChunkDataIndex(cc, chunkDataIndex));
            }

            using (var fs = new FileStream(DataFilePath, FileMode.Append, FileAccess.Write))
            {
                MessagePackSerializer.Serialize(fs, newChunkData);
            }

            return newChunkData;
        }

        private ChunkData ReadChunk(long index)
        {
            using (var fs = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read))
            {
                fs.Position = ChunkDataByteSize * index;
                using (var br = new BinaryReader(fs))
                {
                    var bytes = br.ReadBytes(ChunkDataByteSize);
                    return MessagePackSerializer.Deserialize<ChunkData>(bytes);
                }
            }
        }
    }

    [MessagePackObject]
    public struct ChunkDataIndex
    {
        [Key(0)]
        public ChunkCoordinate ChunkCoordinate;
        [Key(1)]
        public long Index;

        public ChunkDataIndex(ChunkCoordinate cc, long index)
        {
            ChunkCoordinate = cc;
            Index = index;
        }
    }
}
