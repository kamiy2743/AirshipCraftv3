using System;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のチャンクの座標
    /// 座標というよりはインデックスに近い
    /// </summary>
    internal struct ChunkCoordinate : IEquatable<ChunkCoordinate>
    {
        internal readonly int x;
        internal readonly int y;
        internal readonly int z;

        internal ChunkCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, y)) throw new System.Exception($"chunk({x}, {y}, {z}) is invalid");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static bool IsValid(int x, int y, int z)
        {
            if (x < 0 || x >= World.WorldChunkSideXZ) return false;
            if (y < 0 || y >= World.WorldChunkSideY) return false;
            if (z < 0 || z >= World.WorldChunkSideXZ) return false;
            return true;
        }

        private const float InverseBlockSide = 1f / World.ChunkBlockSide;
        internal static ChunkCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new ChunkCoordinate(
                // BlockSideで割り算
                (int)(bc.x * InverseBlockSide),
                (int)(bc.y * InverseBlockSide),
                (int)(bc.z * InverseBlockSide)
            );
        }

        public override string ToString()
        {
            return $"Chunk({x}, {y}, {z})";
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoordinate data && Equals(data);
        }

        public bool Equals(ChunkCoordinate other)
        {
            if (this.x != other.x) return false;
            if (this.y != other.y) return false;
            if (this.z != other.z) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.x, this.y, this.z);
        }

        public static bool operator ==(ChunkCoordinate left, ChunkCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkCoordinate left, ChunkCoordinate right)
        {
            return !(left == right);
        }
    }
}
