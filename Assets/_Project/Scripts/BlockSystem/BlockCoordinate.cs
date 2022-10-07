using UnityEngine;
using System;
using MessagePack;
using Unity.Mathematics;

namespace BlockSystem
{
    /// <summary>
    /// ワールド内のブロックの座標
    /// </summary>
    [MessagePackObject]
    public struct BlockCoordinate : IEquatable<BlockCoordinate>
    {
        [Key(0)]
        public readonly int x;
        [Key(1)]
        public readonly int y;
        [Key(2)]
        public readonly int z;

        public static readonly Vector3Int Max = ChunkCoordinate.Max * ChunkData.ChunkBlockSide;
        public static readonly Vector3Int Min = ChunkCoordinate.Min * ChunkData.ChunkBlockSide;

        internal Vector3 Center => ToVector3() + (Vector3.one * 0.5f);

        internal BlockCoordinate(Vector3 position) : this((int)math.floor(position.x), (int)math.floor(position.y), (int)math.floor(position.z)) { }

        /// <param name="ignoreValidation">パフォーマンス追及以外の用途では絶対に使用しないでください</param>
        public BlockCoordinate(int x, int y, int z, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
            {
                if (!IsValid(x, y, z)) throw new System.Exception($"block({x}, {y}, {z}) is invalid");
            }

            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static bool IsValid(Vector3 position)
        {
            return IsValid((int)position.x, (int)position.y, (int)position.z);
        }
        internal static bool IsValid(int x, int y, int z)
        {
            if (x < Min.x || x > Max.x) return false;
            if (y < Min.y || y > Max.y) return false;
            if (z < Min.z || z > Max.z) return false;
            return true;
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
