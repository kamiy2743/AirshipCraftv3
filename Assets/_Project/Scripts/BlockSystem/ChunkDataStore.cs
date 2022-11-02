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

        private const int CacheCapacity = 128;
        private Hashtable chunkDataCache = new Hashtable(CacheCapacity);

        private HashSet<ChunkData> reusableChunkHashSet = new HashSet<ChunkData>();
        private List<ChunkData> reusableChunkList = new List<ChunkData>();

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
            if (ct.IsCancellationRequested) return null;

            lock (chunkDataCache)
            {
                // キャッシュにあればそれを返す
                if (chunkDataCache.ContainsKey(cc))
                {
                    var cache = (ChunkData)chunkDataCache[cc];

                    // 再利用リストにあれば削除
                    TryRemoveReusableChunk(cache);
                    cache.ReferenceCounter.AddRef();

                    return cache;
                }

                // 再利用可能チャンクの取得
                var useReusableChunkData = TryGetReusableChunkData(out ChunkData reusableChunkData);

                ChunkData chunkData;
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
                // キャッシュに追加
                chunkDataCache.Add(cc, chunkData);

                // 新規作成時のみ購読
                if (!useReusableChunkData)
                {
                    // 参照がなくなったら再利用リストに追加
                    chunkData.ReferenceCounter.OnAllReferenceReleased.Subscribe(_ => AddReusableChunk(chunkData));
                }

                return chunkData;
            }
        }

        /// <summary>
        /// 再利用可能なチャンクの取得
        /// </summary>
        private bool TryGetReusableChunkData(out ChunkData reusableChunkData)
        {
            // キャッシュが埋まってなければ再利用はしない
            if (chunkDataCache.Count < CacheCapacity)
            {
                reusableChunkData = null;
                return false;
            }

            if (reusableChunkList.Count > 0)
            {
                // 先頭を返す
                reusableChunkData = reusableChunkList[0];
                reusableChunkList.RemoveAt(0);
                reusableChunkHashSet.Remove(reusableChunkData);
                chunkDataCache.Remove(reusableChunkData.ChunkCoordinate);
                return true;
            }

            reusableChunkData = null;
            return false;
        }

        /// <summary>
        /// 再利用可能チャンクに追加
        /// </summary>
        private void AddReusableChunk(ChunkData addChunk)
        {
            lock (this)
            {
                reusableChunkHashSet.Add(addChunk);
                reusableChunkList.Add(addChunk);
            }
        }

        /// <summary>
        /// 再利用可能チャンクから削除
        /// </summary>
        private bool TryRemoveReusableChunk(ChunkData removeChunk)
        {
            if (reusableChunkHashSet.Contains(removeChunk))
            {
                reusableChunkHashSet.Remove(removeChunk);
                reusableChunkList.Remove(removeChunk);
                return true;
            }

            return false;
        }

        /// <summary>
        /// チャンクを新規作成し、チャンクとその保存位置を書き込む
        /// </summary>
        private ChunkData CreateNewChunk(ChunkCoordinate cc, ChunkData reusableChunkData)
        {
            ChunkData newChunkData;

            // 再利用可能なChunkDataがあれば再利用する
            if (reusableChunkData is not null)
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
