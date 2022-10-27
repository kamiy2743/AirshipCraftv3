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
        /// <summary> チャンクの保存位置を格納する </summary>
        private Hashtable indexHashtable = new Hashtable();
        private long createdChunkCount = 0;

        private List<ChunkData> allocatedChunkData = new List<ChunkData>();

        private MapGenerator _mapGenerator;

        private static readonly string RootDirectory = Application.persistentDataPath + "/" + nameof(ChunkDataStore);
        private static readonly string ChunkDataFilePath = RootDirectory + "/ChunkData.bin";
        private static readonly string IndexHashtablePath = RootDirectory + "/IndexHashtable.bin";

        private readonly FileStream ChunkDataStream;
        private readonly FileStream IndexHashtableStream;

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;

            Directory.CreateDirectory(RootDirectory);
            ChunkDataStream = new FileStream(ChunkDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IndexHashtableStream = new FileStream(IndexHashtablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            // チャンクの保存位置を読み込む
            var indexHashtableBinaryReader = new BinaryReader(IndexHashtableStream);
            var chunkDataIndexByteSize = MessagePackSerializer.Serialize(new ChunkDataIndex()).Length;

            while (IndexHashtableStream.Position < IndexHashtableStream.Length)
            {
                var bytes = indexHashtableBinaryReader.ReadBytes(chunkDataIndexByteSize);
                var chunkDataIndex = MessagePackSerializer.Deserialize<ChunkDataIndex>(bytes);
                indexHashtable.Add(chunkDataIndex.ChunkCoordinate, chunkDataIndex.Index);
                createdChunkCount++;
            }

            indexHashtableBinaryReader.Dispose();
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
        /// 終わったらChunkDataの参照を開放することを忘れないように
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            lock (indexHashtable)
            {
                ChunkData chunkData;

                // 保存位置が存在すれば読み込み、無ければ作成
                if (indexHashtable.ContainsKey(cc))
                {
                    chunkData = ReadChunk((long)indexHashtable[cc]);
                }
                else
                {
                    chunkData = CreateNewChunk(cc);
                }

                if (ct.IsCancellationRequested) return null;
                chunkData.ReferenceCounter.AddRef();
                return chunkData;
            }
        }

        /// <summary>
        /// チャンクを新規作成し、チャンクとその保存位置を書き込む
        /// </summary>
        private ChunkData CreateNewChunk(ChunkCoordinate cc)
        {
            ChunkData newChunkData;

            // 再利用可能なChunkDataがあれば再利用する
            if (TryGetReusableChunkData(out var reusableChunkData))
            {
                newChunkData = reusableChunkData.ReuseConstructor(cc, _mapGenerator);
            }
            else
            {
                newChunkData = ChunkData.NewConstructor(cc, _mapGenerator);
                allocatedChunkData.Add(newChunkData);
            }

            // チャンクの保存位置を書き込む
            IndexHashtableStream.Position = IndexHashtableStream.Length;
            MessagePackSerializer.Serialize(IndexHashtableStream, new ChunkDataIndex(cc, createdChunkCount));
            indexHashtable.Add(cc, createdChunkCount);
            createdChunkCount++;

            // チャンク本体を書き込む
            var bytes = ChunkDataSerializer.Serialize(newChunkData);
            ChunkDataStream.Position = ChunkDataStream.Length;
            ChunkDataStream.Write(bytes);

            return newChunkData;
        }

        /// <summary>
        /// 保存されているチャンクを読み込む
        /// </summary>
        private byte[] readBuffer = new byte[ChunkDataSerializer.ChunkDataByteSize];
        private ChunkData ReadChunk(long index)
        {
            ChunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * index;
            ChunkDataStream.Read(readBuffer, 0, ChunkDataSerializer.ChunkDataByteSize);

            if (TryGetReusableChunkData(out var reusableChunkData))
            {
                return ChunkDataSerializer.Deserialize(readBuffer, reusableChunkData);
            }
            else
            {
                var chunkData = ChunkDataSerializer.Deserialize(readBuffer);
                allocatedChunkData.Add(chunkData);
                return chunkData;
            }
        }

        /// <summary>
        /// 再利用可能なChunkDataを取得する
        /// </summary>
        private bool TryGetReusableChunkData(out ChunkData reusableChunkData)
        {
            foreach (var chunkData in allocatedChunkData)
            {
                if (chunkData.ReferenceCounter.IsFree)
                {
                    reusableChunkData = chunkData;
                    return true;
                }
            }

            reusableChunkData = null;
            return false;
        }

        public void Dispose()
        {
            ChunkDataStream.Dispose();
            IndexHashtableStream.Dispose();
        }
    }

    /// <summary>
    /// 保存位置を表す
    /// </summary>
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
