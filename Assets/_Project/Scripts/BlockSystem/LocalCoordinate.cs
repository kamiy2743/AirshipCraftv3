using MessagePack;

namespace BlockSystem
{
    /// <summary>
    /// チャンク内のブロックの座標
    public struct LocalCoordinate
    {
        public readonly byte x;
        public readonly byte y;
        public readonly byte z;

        /// <summary>
        /// シリアライズ用
        /// </summary>
        public LocalCoordinate(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal LocalCoordinate(int x, int y, int z)
        {
            if (x < 0 || x >= World.ChunkBlockSide) throw new System.Exception("ローカルx座標が不正です: " + x);
            if (y < 0 || y >= World.ChunkBlockSide) throw new System.Exception("ローカルy座標が不正です: " + y);
            if (z < 0 || z >= World.ChunkBlockSide) throw new System.Exception("ローカルz座標が不正です: " + z);

            this.x = (byte)x;
            this.y = (byte)y;
            this.z = (byte)z;
        }

        private const float InverseBlockSide = 1f / World.ChunkBlockSide;
        public static LocalCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new LocalCoordinate(
                // BlockSideで割った余り
                (int)bc.x - ((int)(bc.x * InverseBlockSide) * World.ChunkBlockSide),
                (int)bc.y - ((int)(bc.y * InverseBlockSide) * World.ChunkBlockSide),
                (int)bc.z - ((int)(bc.z * InverseBlockSide) * World.ChunkBlockSide)
            );
        }
    }
}
