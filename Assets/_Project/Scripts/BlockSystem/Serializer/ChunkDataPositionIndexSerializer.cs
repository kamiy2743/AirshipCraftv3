using DataObject.Chunk;

namespace BlockSystem.Serializer
{
    internal static class ChunkDataPositionIndexSerializer
    {
        // ChunkCoordinateのバイト数
        private const int ChunkCoordinateByteSize = sizeof(short) * 3;
        // Indexのバイト数
        private const int IndexByteSize = sizeof(long);

        // ChunkDataPositionIndexのバイト数
        internal const int ChunkDataPositionIndexByteSize = ChunkCoordinateByteSize + IndexByteSize;

        private static byte[] writeBuffer = new byte[ChunkDataPositionIndexByteSize];
        internal static byte[] Serialize(ChunkDataPositionIndex chunkDataPositionIndex)
        {
            var offset = 0;

            // TODO ChunkCoordinateの共通化
            var cc = chunkDataPositionIndex.ChunkCoordinate;
            writeBuffer[offset++] = (byte)(cc.x >> 8);
            writeBuffer[offset++] = (byte)cc.x;
            writeBuffer[offset++] = (byte)(cc.y >> 8);
            writeBuffer[offset++] = (byte)cc.y;
            writeBuffer[offset++] = (byte)(cc.z >> 8);
            writeBuffer[offset++] = (byte)cc.z;

            var index = chunkDataPositionIndex.Index;
            for (int i = 0; i < IndexByteSize; i++)
            {
                writeBuffer[offset++] = (byte)(index >> (8 * (IndexByteSize - 1 - i)));
            }

            return writeBuffer;
        }

        internal static ChunkDataPositionIndex Deserialize(byte[] bytes)
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

            return new ChunkDataPositionIndex(cc, index);
        }
    }
}
