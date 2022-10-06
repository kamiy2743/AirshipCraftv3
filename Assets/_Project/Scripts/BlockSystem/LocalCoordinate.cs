using Unity.Mathematics;

namespace BlockSystem
{
    /// <summary>
    /// チャンク内のブロックの座標
    // TODO LocalCoordinateいらないかも
    public struct LocalCoordinate
    {
        public readonly byte x;
        public readonly byte y;
        public readonly byte z;

        public LocalCoordinate(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        private const byte mask = (byte)(ChunkData.ChunkBlockSide - 1);
        public static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                (byte)(bc.x & mask),
                (byte)(bc.y & mask),
                (byte)(bc.z & mask)
            );
        }

        public override string ToString()
        {
            return $"Local({x}, {y}, {z})";
        }
    }
}
