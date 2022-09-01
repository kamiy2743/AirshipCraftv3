namespace BlockSystem
{
    /// <summary>
    /// チャンク内のブロックの座標
    /// </summary>
    public class LocalCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        private const int BlockSide = WorldSettings.LocalBlockSide;

        public LocalCoordinate(int x, int y, int z)
        {
            if (x < 0 || x >= BlockSide) throw new System.Exception("ローカルx座標が不正です: " + x);
            if (y < 0 || y >= BlockSide) throw new System.Exception("ローカルy座標が不正です: " + y);
            if (z < 0 || z >= BlockSide) throw new System.Exception("ローカルz座標が不正です: " + z);

            this.x = x;
            this.y = y;
            this.z = z;
        }

        private const float InverseBlockSide = 1f / BlockSide;
        public static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                // BlockSideで割った余り
                bc.x - ((int)(bc.x * InverseBlockSide) * BlockSide),
                bc.y - ((int)(bc.y * InverseBlockSide) * BlockSide),
                bc.z - ((int)(bc.z * InverseBlockSide) * BlockSide)
            );
        }
    }
}
