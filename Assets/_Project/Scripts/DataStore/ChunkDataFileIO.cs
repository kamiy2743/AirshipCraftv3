using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DataStore.Serializer;
using DataObject.Chunk;
using System.Threading;

namespace DataStore
{
    public class ChunkDataFileIO : IDisposable
    {
        /// <summary> チャンクの保存位置を格納する </summary>
        private Dictionary<ChunkCoordinate, long> positionIndexDictionary = new Dictionary<ChunkCoordinate, long>();
        private long createdChunkCount = 0;

        private static readonly string RootDirectory = Application.persistentDataPath + "/" + nameof(ChunkDataStore);
        private static readonly string ChunkDataFilePath = RootDirectory + "/ChunkData.bin";
        private static readonly string PositionIndexFilePath = RootDirectory + "/PositionIndex.bin";

        private readonly FileStream chunkDataStream;
        private readonly FileStream positionIndexStream;

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public ChunkDataFileIO()
        {
            Directory.CreateDirectory(RootDirectory);
            chunkDataStream = new FileStream(ChunkDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            positionIndexStream = new FileStream(PositionIndexFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            // chunkDataStream.SetLength(0);
            // positionIndexStream.SetLength(0);

            // チャンクの保存位置を読み込む
            var readBuffer = new byte[ChunkDataPositionIndexSerializer.ChunkDataPositionIndexByteSize];
            while (positionIndexStream.Position < positionIndexStream.Length)
            {
                positionIndexStream.Read(readBuffer, 0, readBuffer.Length);
                var positionIndex = ChunkDataPositionIndexSerializer.Deserialize(readBuffer);
                positionIndexDictionary.Add(positionIndex.ChunkCoordinate, positionIndex.Index);
                createdChunkCount++;
            }
        }

        private void Write(long positionIndex, ChunkData chunkData)
        {
            var isHeld = rwLock.IsWriteLockHeld;
            if (!isHeld) rwLock.EnterWriteLock();

            var chunkDataBytes = ChunkDataSerializer.Serialize(chunkData);
            chunkDataStream.Position = positionIndex * ChunkDataSerializer.ChunkDataByteSize;
            chunkDataStream.Write(chunkDataBytes);

            if (!isHeld) rwLock.ExitWriteLock();
        }

        internal void Add(ChunkData chunkData)
        {
            rwLock.EnterWriteLock();

            // 保存位置のインデックス
            var positionIndex = createdChunkCount;

            // チャンクの保存位置を書き込む
            var positionIndexBytes = ChunkDataPositionIndexSerializer.Serialize(new ChunkDataPositionIndex(chunkData.ChunkCoordinate, positionIndex));
            positionIndexStream.Write(positionIndexBytes);
            positionIndexDictionary.Add(chunkData.ChunkCoordinate, positionIndex);

            // チャンク本体を書き込む
            Write(positionIndex, chunkData);
            createdChunkCount++;

            rwLock.ExitWriteLock();
        }

        public void AddOrUpdate(ChunkData chunkData)
        {
            rwLock.EnterReadLock();

            var existPosition = positionIndexDictionary.TryGetValue(chunkData.ChunkCoordinate, out var positionIndex);

            rwLock.ExitReadLock();

            if (existPosition)
            {
                Write(positionIndex, chunkData);
            }
            else
            {
                Add(chunkData);
            }
        }

        private static byte[] readBuffer = new byte[ChunkDataSerializer.ChunkDataByteSize];
        internal bool Read(ChunkCoordinate cc, out ChunkData result) => Read(cc, null, out result);
        internal bool Read(ChunkCoordinate cc, ChunkData reusableChunkData, out ChunkData result)
        {
            rwLock.EnterReadLock();
            try
            {
                if (!positionIndexDictionary.TryGetValue(cc, out var positionIndex))
                {
                    result = null;
                    return false;
                }

                chunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * positionIndex;
                chunkDataStream.Read(readBuffer, 0, readBuffer.Length);
                result = ChunkDataSerializer.Deserialize(readBuffer, reusableChunkData);
                return true;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            chunkDataStream.Dispose();
            positionIndexStream.Dispose();
            rwLock.Dispose();
        }
    }

    /// <summary> 保存位置を表す </summary>
    internal struct ChunkDataPositionIndex
    {
        internal ChunkCoordinate ChunkCoordinate;
        internal long Index;

        internal ChunkDataPositionIndex(ChunkCoordinate cc, long index)
        {
            ChunkCoordinate = cc;
            Index = index;
        }
    }
}
