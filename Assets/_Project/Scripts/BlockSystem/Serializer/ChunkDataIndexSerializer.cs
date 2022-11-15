using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using MasterData.Block;
using Util;

namespace BlockSystem.Serializer
{
    internal static class ChunkDataIndexSerializer
    {
        // ChunkCoordinateのバイト数
        private const int ChunkCoordinateByteSize = 6; //int16*3
        // Indexのバイト数
        private const int IndexByteSize = 8; //int64

        // ChunkDataIndexのバイト数
        internal const int ChunkDataIndexByteSize = ChunkCoordinateByteSize + IndexByteSize;

        private static byte[] writeBuffer = new byte[ChunkDataIndexByteSize];
        internal static byte[] Serialize(ChunkDataIndex chunkDataIndex)
        {
            var offset = 0;

            // TODO ChunkCoordinateの共通化
            var cc = chunkDataIndex.ChunkCoordinate;
            writeBuffer[offset++] = (byte)(cc.x >> 8);
            writeBuffer[offset++] = (byte)cc.x;
            writeBuffer[offset++] = (byte)(cc.y >> 8);
            writeBuffer[offset++] = (byte)cc.y;
            writeBuffer[offset++] = (byte)(cc.z >> 8);
            writeBuffer[offset++] = (byte)cc.z;

            var index = chunkDataIndex.Index;
            for (int i = 0; i < IndexByteSize; i++)
            {
                writeBuffer[offset++] = (byte)(index >> (8 * (IndexByteSize - 1 - i)));
            }

            return writeBuffer;
        }

        internal static ChunkDataIndex Deserialize(byte[] bytes)
        {
            var offset = 0;

            var ccx = (bytes[offset++] << 8) + bytes[offset++];
            var ccy = (bytes[offset++] << 8) + bytes[offset++];
            var ccz = (bytes[offset++] << 8) + bytes[offset++];
            var cc = new ChunkCoordinate((short)ccx, (short)ccy, (short)ccz);

            var index = 0;
            for (int i = 0; i < IndexByteSize; i++)
            {
                index += bytes[offset++] << (8 * (IndexByteSize - 1 - i));
            }

            return new ChunkDataIndex(cc, index);
        }
    }
}
