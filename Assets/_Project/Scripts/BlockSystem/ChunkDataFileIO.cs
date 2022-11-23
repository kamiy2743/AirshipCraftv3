using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockSystem.Serializer;
using UniRx;


namespace BlockSystem
{
    internal class ChunkDataFileIO : IDisposable
    {
        /// <summary> チャンクの保存位置を格納する </summary>
        private Dictionary<ChunkCoordinate, long> positionIndexDictionary = new Dictionary<ChunkCoordinate, long>();
        private long createdChunkCount = 0;

        private static readonly string RootDirectory = Application.persistentDataPath + "/" + nameof(ChunkDataStore);
        private static readonly string ChunkDataFilePath = RootDirectory + "/ChunkData.bin";
        private static readonly string PositionIndexFilePath = RootDirectory + "/PositionIndex.bin";

        private readonly FileStream chunkDataStream;
        private readonly FileStream positionIndexStream;

        internal ChunkDataFileIO()
        {
            Directory.CreateDirectory(RootDirectory);
            chunkDataStream = new FileStream(ChunkDataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            positionIndexStream = new FileStream(PositionIndexFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            chunkDataStream.SetLength(0);
            positionIndexStream.SetLength(0);

            // チャンクの保存位置を読み込む
            var readBuffer = new byte[ChunkDataPositionIndexSerializer.ChunkDataPositionIndexByteSize];
            while (positionIndexStream.Position < positionIndexStream.Length)
            {
                positionIndexStream.Read(readBuffer, 0, readBuffer.Length);
                var chunkDataPositionIndex = ChunkDataPositionIndexSerializer.Deserialize(readBuffer);
                positionIndexDictionary.Add(chunkDataPositionIndex.ChunkCoordinate, chunkDataPositionIndex.Index);
                createdChunkCount++;
            }
        }

        internal void Append(ChunkData chunkData)
        {
            // チャンクの保存位置を書き込む
            var chunkDataPositionIndexBytes = ChunkDataPositionIndexSerializer.Serialize(new ChunkDataPositionIndex(chunkData.ChunkCoordinate, createdChunkCount));
            positionIndexStream.Write(chunkDataPositionIndexBytes);
            positionIndexDictionary.Add(chunkData.ChunkCoordinate, createdChunkCount);

            // チャンク本体を書き込む
            var chunkDataBytes = ChunkDataSerializer.Serialize(chunkData);
            chunkDataStream.Position = createdChunkCount * ChunkDataSerializer.ChunkDataByteSize;
            chunkDataStream.Write(chunkDataBytes);
            createdChunkCount++;
        }

        private byte[] readBuffer = new byte[ChunkDataSerializer.ChunkDataByteSize];
        internal bool TryRead(ChunkCoordinate cc, out ChunkData result) => TryRead(cc, null, out result);
        internal bool TryRead(ChunkCoordinate cc, ChunkData reusableChunkData, out ChunkData result)
        {
            if (!positionIndexDictionary.TryGetValue(cc, out var index))
            {
                result = null;
                return false;
            }

            chunkDataStream.Position = ChunkDataSerializer.ChunkDataByteSize * index;
            chunkDataStream.Read(readBuffer, 0, readBuffer.Length);
            result = ChunkDataSerializer.Deserialize(readBuffer, reusableChunkData);
            return true;
        }

        public void Dispose()
        {
            chunkDataStream.Dispose();
            positionIndexStream.Dispose();
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
