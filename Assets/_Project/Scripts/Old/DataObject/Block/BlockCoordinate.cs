using UnityEngine;
using System;
using Unity.Mathematics;
using DataObject.Chunk;

namespace DataObject.Block
{
    /// <summary>
    /// ブロックの座標
    /// ワールド内のグローバル座標
    /// </summary>
    public struct BlockCoordinate : IEquatable<BlockCoordinate>
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public Vector3 Center => new Vector3(x, y, z) + Vector3.one * 0.5f;

        internal const int Max = ChunkCoordinate.Max * ChunkData.ChunkBlockSide;
        internal const int Min = ChunkCoordinate.Min * ChunkData.ChunkBlockSide;

        public BlockCoordinate(Vector3 position) : this((int)math.floor(position.x), (int)math.floor(position.y), (int)math.floor(position.z)) { }
        public BlockCoordinate(int3 position) : this(position.x, position.y, position.z) { }
        public BlockCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z)) throw new System.Exception($"block({x}, {y}, {z}) is invalid");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static bool IsValid(Vector3 position) => IsValid((int)math.floor(position.x), (int)math.floor(position.y), (int)math.floor(position.z));
        public static bool IsValid(int3 position) => IsValid(position.x, position.y, position.z);
        public static bool IsValid(int x, int y, int z)
        {
            if (x < Min || x > Max) return false;
            if (y < Min || y > Max) return false;
            if (z < Min || z > Max) return false;
            return true;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public int3 ToInt3()
        {
            return new int3(x, y, z);
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
