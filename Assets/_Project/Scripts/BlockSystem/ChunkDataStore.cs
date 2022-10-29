using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using BlockSystem.Serializer;
using UniRx;

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

        private Queue<ChunkData> reusableChunkQueue = new Queue<ChunkData>();

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
            var chunkDataIndexByteSize = MessagePackSerializer.Serialize(new ChunkDataIndex()).Length;
            var readBuffer = new byte[chunkDataIndexByteSize];
            while (IndexHashtableStream.Position < IndexHashtableStream.Length)
            {
                IndexHashtableStream.Read(readBuffer, 0, readBuffer.Length);
                var chunkDataIndex = MessagePackSerializer.Deserialize<ChunkDataIndex>(readBuffer);
                indexHashtable.Add(chunkDataIndex.ChunkCoordinate, chunkDataIndex.Index);
                createdChunkCount++;
            }
        }

        /// <summary>
        /// チャンクデータ取得、無ければ作成
        /// キャンセルされた場合はnullを返す
        /// 使い終わったら参照を開放することを忘れないように
        /// </summary>
        internal ChunkData GetChunkData(ChunkCoordinate cc, CancellationToken ct)
        {
            lock (indexHashtable)
            {
                ChunkData chunkData;
                reusableChunkQueue.TryDequeue(out ChunkData reusableChunkData);

                // 保存位置が存在すれば読み込み、無ければ作成
                if (indexHashtable.ContainsKey(cc))
                {
                    chunkData = ReadChunk((long)indexHashtable[cc], reusableChunkData);
                }
                else
                {
                    chunkData = CreateNewChunk(cc, reusableChunkData);
                }

                if (ct.IsCancellationRequested) return null;

                // 参照追加
                chunkData.ReferenceCounter.AddRef();
                // 参照がなくなったら再利用キューに追加
                if (reusableChunkData == null)
                {
                    chunkData.ReferenceCounter.OnAllReferenceReleased.Subscribe(_ => reusableChunkQueue.Enqueue(chunkData));
                }

                return chunkData;
            }
        }

        /// <summary>
        /// チャンクを新規作成し、チャンクとその保存位置を書き込む
        /// </summary>
        private ChunkData CreateNewChunk(ChunkCoordinate cc, ChunkData reusableChunkData)
        {
            ChunkData newChunkData;

            // 再利用可能なChunkDataがあれば再利用する
            if (reusableChunkData != null)
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
        private byte[] readBuffer = new byte[ChunkDataSerializer.ChunkDataByteSize];
        private ChunkData ReadChunk(long index, ChunkData reusableChunkData)
        {
            ChunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * index;
            ChunkDataStream.Read(readBuffer, 0, readBuffer.Length);
            return ChunkDataSerializer.Deserialize(readBuffer, reusableChunkData);
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
