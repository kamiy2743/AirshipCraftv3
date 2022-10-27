namespace BlockSystem
{
    /// <summary>
    /// チャンク内のブロックの座標
    // TODO LocalCoordinateいらないかも
    internal struct LocalCoordinate
    {
        internal readonly byte x;
        internal readonly byte y;
        internal readonly byte z;

        private LocalCoordinate(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        private const byte mask = (byte)(ChunkData.ChunkBlockSide - 1);
        internal static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
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
