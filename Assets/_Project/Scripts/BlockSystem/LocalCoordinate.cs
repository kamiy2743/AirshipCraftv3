namespace BlockSystem
{
    /// <summary>
    /// チャンク内のブロックの座標
    /// </summary>
    internal struct LocalCoordinate
    {
        internal readonly int x;
        internal readonly int y;
        internal readonly int z;

        internal LocalCoordinate(int x, int y, int z)
        {
            if (x < 0 || x >= World.ChunkBlockSide) throw new System.Exception("ローカルx座標が不正です: " + x);
            if (y < 0 || y >= World.ChunkBlockSide) throw new System.Exception("ローカルy座標が不正です: " + y);
            if (z < 0 || z >= World.ChunkBlockSide) throw new System.Exception("ローカルz座標が不正です: " + z);

            this.x = x;
            this.y = y;
            this.z = z;
        }

        private const float InverseBlockSide = 1f / World.ChunkBlockSide;
        internal static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                // BlockSideで割った余り
                bc.x - ((int)(bc.x * InverseBlockSide) * World.ChunkBlockSide),
                bc.y - ((int)(bc.y * InverseBlockSide) * World.ChunkBlockSide),
                bc.z - ((int)(bc.z * InverseBlockSide) * World.ChunkBlockSide)
            );
        }
    }
}
