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

        public LocalCoordinate(int x, int y, int z)
        {
            if (x < 0 || x >= WorldSettings.LocalBlockSide) throw new System.Exception("ローカルx座標が不正です: " + x);
            if (y < 0 || y >= WorldSettings.LocalBlockSide) throw new System.Exception("ローカルy座標が不正です: " + y);
            if (z < 0 || z >= WorldSettings.LocalBlockSide) throw new System.Exception("ローカルz座標が不正です: " + z);

            this.x = x;
            this.y = y;
            this.z = z;
        }

        private const float InverseBlockSide = 1f / WorldSettings.LocalBlockSide;
        public static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                // BlockSideで割った余り
                bc.x - ((int)(bc.x * InverseBlockSide) * WorldSettings.LocalBlockSide),
                bc.y - ((int)(bc.y * InverseBlockSide) * WorldSettings.LocalBlockSide),
                bc.z - ((int)(bc.z * InverseBlockSide) * WorldSettings.LocalBlockSide)
            );
        }
    }
}
