using UnityEngine;
using System;
using MessagePack;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のブロックの座標
    /// </summary>
    [MessagePackObject]
    public struct BlockCoordinate : IEquatable<BlockCoordinate>
    {
        [Key(0)]
        public readonly uint x;
        [Key(1)]
        public readonly uint y;
        [Key(2)]
        public readonly uint z;

        internal Vector3 Center => ToVector3() + (Vector3.one * 0.5f);

        [SerializationConstructor]
        public BlockCoordinate(uint x, uint y, uint z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal BlockCoordinate(Vector3 position) : this((int)position.x, (int)position.y, (int)position.z) { }
        internal BlockCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z)) throw new System.Exception($"block({x}, {y}, {z}) is invalid");

            this.x = (uint)x;
            this.y = (uint)y;
            this.z = (uint)z;
        }

        internal static bool IsValid(Vector3 position)
        {
            return IsValid((int)position.x, (int)position.y, (int)position.z);
        }
        internal static bool IsValid(int x, int y, int z)
        {
            if (x < 0 || x >= World.WorldBlockSideXZ) return false;
            if (y < 0 || y >= World.WorldBlockSideY) return false;
            if (z < 0 || z >= World.WorldBlockSideXZ) return false;
            return true;
        }

        public static BlockCoordinate FromChunkAndLocal(ChunkCoordinate cc, LocalCoordinate lc)
        {
            return new BlockCoordinate(
                cc.x * World.ChunkBlockSide + lc.x,
                cc.y * World.ChunkBlockSide + lc.y,
                cc.z * World.ChunkBlockSide + lc.z
            );
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"Block({x}, {y}, {z})";
        }

        public override bool Equals(object obj)
        {
            return obj is BlockCoordinate data && Equals(data);
        }

        public bool Equals(BlockCoordinate other)
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

        public static bool operator ==(BlockCoordinate left, BlockCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockCoordinate left, BlockCoordinate right)
        {
            return !(left == right);
        }
    }
}
