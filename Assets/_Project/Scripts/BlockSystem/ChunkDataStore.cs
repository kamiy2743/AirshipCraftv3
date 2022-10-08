using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using BlockSystem.Serializer;

namespace BlockSystem
{
    /// <summary>
    /// チャンクデータを管理
    /// </summary>
    internal class ChunkDataStore : IDisposable
    {
        private const int chunkDataCacheCapacity = 512;
        private Hashtable chunkDataCache = new Hashtable(chunkDataCacheCapacity);
        private Queue<ChunkCoordinate> chunkDataCacheQueue = new Queue<ChunkCoordinate>(chunkDataCacheCapacity);

        private Hashtable indexHashtable = new Hashtable();

        private MapGenerator _mapGenerator;

        private static readonly string ChunkDataFilePath = Application.persistentDataPath + "/ChunkDataStore/ChunkData.bin";
        private static readonly string IndexHashtablePath = Application.persistentDataPath + "/ChunkDataStore/IndexHashtable.bin";

        private static readonly FileStream ChunkDataStream = new FileStream(ChunkDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        private static readonly FileStream IndexHashtableStream = new FileStream(IndexHashtablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        private static readonly BinaryReader ChunkDataBinaryReader = new BinaryReader(ChunkDataStream);
        private static readonly BinaryReader IndexHashtableBinaryReader = new BinaryReader(IndexHashtableStream);

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;

            var chunkDataIndexByteSize = MessagePackSerializer.Serialize(new ChunkDataIndex()).Length;
            while (IndexHashtableStream.Position < IndexHashtableStream.Length)
            {
                var bytes = IndexHashtableBinaryReader.ReadBytes(chunkDataIndexByteSize);
                var chunkDataIndex = MessagePackSerializer.Deserialize<ChunkDataIndex>(bytes);
                indexHashtable.Add(chunkDataIndex.ChunkCoordinate, chunkDataIndex.Index);
            }
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            if (chunkDataCache.ContainsKey(cc))
            {
                return (ChunkData)chunkDataCache[cc];
            }

            if (ct.IsCancellationRequested) return null;

            lock (chunkDataCache)
            {
                ChunkData chunkData = null;

                if (indexHashtable.ContainsKey(cc))
                {
                    chunkData = ReadChunk((long)indexHashtable[cc]);
                    if (chunkData == null) return null;
                }
                else
                {
                    chunkData = CreateNewChunk(cc);
                }

                if (chunkDataCache.Count >= chunkDataCacheCapacity)
                {
                    var releaseChunk = chunkDataCacheQueue.Dequeue();
                    chunkDataCache.Remove(releaseChunk);
                }

                chunkDataCache.Add(cc, chunkData);
                chunkDataCacheQueue.Enqueue(cc);

                if (ct.IsCancellationRequested) return null;

                return chunkData;
            }
        }

        private ChunkData CreateNewChunk(ChunkCoordinate cc)
        {
            var newChunkData = new ChunkData(cc, _mapGenerator);

            // TODO 事実上intの最大値までしか正常に動かない
            var chunkDataIndex = (long)indexHashtable.Count;
            indexHashtable.Add(cc, chunkDataIndex);

            IndexHashtableStream.Position = IndexHashtableStream.Length;
            MessagePackSerializer.Serialize(IndexHashtableStream, new ChunkDataIndex(cc, chunkDataIndex));

            var bytes = ChunkDataSerializer.Serialize(newChunkData);
            ChunkDataStream.Position = ChunkDataStream.Length;
            ChunkDataStream.Write(bytes);

            return newChunkData;
        }

        private ChunkData ReadChunk(long index)
        {
            ChunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * index;
            var bytes = ChunkDataBinaryReader.ReadBytes(ChunkDataSerializer.ChunkDataByteSize);
            return ChunkDataSerializer.Deserialize(bytes);
        }

        public void Dispose()
        {
            ChunkDataStream.Dispose();
            IndexHashtableStream.Dispose();
            ChunkDataBinaryReader.Dispose();
            IndexHashtableBinaryReader.Dispose();
        }
    }

    public struct ChunkDataIndex
    {
        internal ChunkCoordinate ChunkCoordinate;
        internal long Index;

        internal ChunkDataIndex(ChunkCoordinate cc, long index)
        {
            ChunkCoordinate = cc;
            Index = index;
        }
    }
}
