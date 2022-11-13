namespace BlockSystem
{
    /// <summary> チャンク内の座標 </summary>
    internal struct LocalCoordinate
    {
        internal readonly byte x;
        internal readonly byte y;
        internal readonly byte z;

        /// <summary> 
        /// 論理積をとればLocalCoordinateに変換できる 
        /// 極限のパフォーマンスのためのもので、通常<see cref="LocalCoordinate.FromBlockCoordinate(BlockCoordinate)"/>を使う
        /// </summary>
        internal const byte ToLocalCoordinateMask = ChunkData.ChunkBlockSide - 1;

        private LocalCoordinate(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                (byte)(bc.x & ToLocalCoordinateMask),
                (byte)(bc.y & ToLocalCoordinateMask),
                (byte)(bc.z & ToLocalCoordinateMask)
            );
        }

        public override string ToString()
        {
            return $"Local({x}, {y}, {z})";
        }
    }
}
