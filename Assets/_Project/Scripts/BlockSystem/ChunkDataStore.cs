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
        /// <summary> 作成済みチャンクの個数 </summary>
        private long createdChunkCount = 0;

        private MapGenerator _mapGenerator;

        private static readonly string RootDirectory = Application.persistentDataPath + "/" + nameof(ChunkDataStore);
        private static readonly string ChunkDataFilePath = RootDirectory + "/ChunkData.bin";
        private static readonly string IndexHashtablePath = RootDirectory + "/IndexHashtable.bin";

        private readonly FileStream ChunkDataStream;
        private readonly FileStream IndexHashtableStream;
        private readonly BinaryReader ChunkDataBinaryReader;
        private readonly BinaryReader IndexHashtableBinaryReader;

        internal ChunkDataStore(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;

            Directory.CreateDirectory(RootDirectory);
            ChunkDataStream = new FileStream(ChunkDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            IndexHashtableStream = new FileStream(IndexHashtablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            ChunkDataBinaryReader = new BinaryReader(ChunkDataStream);
            IndexHashtableBinaryReader = new BinaryReader(IndexHashtableStream);

            // チャンクの保存位置を読み込む
            var chunkDataIndexByteSize = MessagePackSerializer.Serialize(new ChunkDataIndex()).Length;
            while (IndexHashtableStream.Position < IndexHashtableStream.Length)
            {
                var bytes = IndexHashtableBinaryReader.ReadBytes(chunkDataIndexByteSize);
                var chunkDataIndex = MessagePackSerializer.Deserialize<ChunkDataIndex>(bytes);
                indexHashtable.Add(chunkDataIndex.ChunkCoordinate, chunkDataIndex.Index);
                createdChunkCount++;
            }
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
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
        private ChunkData ReadChunk(long index)
        {
            ChunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * index;
            var bytes = ChunkDataBinaryReader.ReadBytes(ChunkDataSerializer.ChunkDataByteSize);

            if (TryGetReusableChunkData(out var reusableChunkData))
            {
                return ChunkDataSerializer.Deserialize(bytes, reusableChunkData);
            }
            else
            {
                return ChunkDataSerializer.Deserialize(bytes);
            }
        }

        /// <summary>
        /// 再利用可能なChunkDataを取得する
        /// </summary>
        private bool TryGetReusableChunkData(out ChunkData reusableChunkData)
        {
            reusableChunkData = null;
            return false;
        }

        public void Dispose()
        {
            ChunkDataStream.Dispose();
            IndexHashtableStream.Dispose();
            ChunkDataBinaryReader.Dispose();
            IndexHashtableBinaryReader.Dispose();
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
