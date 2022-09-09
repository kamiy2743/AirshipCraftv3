using System;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のチャンクの座標
    /// 座標というよりはインデックスに近い
    /// </summary>
    public struct ChunkCoordinate : IEquatable<ChunkCoordinate>
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
            if (x < 0 || x >= World.WorldChunkSideXZ) return false;
            if (y < 0 || y >= World.WorldChunkSideY) return false;
            if (z < 0 || z >= World.WorldChunkSideXZ) return false;
            return true;
        }

        private const float InverseBlockSide = 1f / World.ChunkBlockSide;
        public static ChunkCoordinate FromBlockCoordinate(BlockCoordinate bc)
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

        public static bool operator ==(ChunkCoordinate cc1, ChunkCoordinate cc2)
        {
            return cc1.Equals(cc2);
        }

        public static bool operator !=(ChunkCoordinate cc1, ChunkCoordinate cc2)
        {
            return !(cc1 == cc2);
        }
    }
}
