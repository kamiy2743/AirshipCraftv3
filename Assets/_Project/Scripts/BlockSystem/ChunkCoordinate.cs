namespace BlockSystem
{
    /// <summary>
    /// ワールド内のチャンクの座標
    /// 座標というよりはインデックスに近い
    /// </summary>
    public class ChunkCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public ChunkCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, y)) throw new System.Exception($"chunk({x}, {y}, {z}) is invalid");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static bool IsValid(int x, int y, int z)
        {
            if (x < 0 || x >= WorldSettings.WorldChunkSideXZ) return false;
            if (y < 0 || y >= WorldSettings.WorldChunkSideY) return false;
            if (z < 0 || z >= WorldSettings.WorldChunkSideXZ) return false;
            return true;
        }

        public static ChunkCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new ChunkCoordinate(
                bc.x / WorldSettings.LocalBlockSide,
                bc.y / WorldSettings.LocalBlockSide,
                bc.z / WorldSettings.LocalBlockSide
            );
        }

        public override string ToString()
        {
            return $"Chunk({x}, {y}, {z})";
        }

        public static bool operator ==(ChunkCoordinate cc1, ChunkCoordinate cc2)
        {
            if (cc1.x != cc2.x) return false;
            if (cc1.y != cc2.y) return false;
            if (cc1.z != cc2.z) return false;
            return true;
        }

        public static bool operator !=(ChunkCoordinate cc1, ChunkCoordinate cc2)
        {
            return !(cc1 == cc2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as ChunkCoordinate;
            return this == other;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(this.x, this.y, this.z);
        }
    }
}
