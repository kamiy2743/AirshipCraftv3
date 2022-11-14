using System;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// ブロックを<see cref="ChunkData.ChunkBlockSide"/>で区切った単位
    /// </summary>
    internal struct ChunkCoordinate : IEquatable<ChunkCoordinate>
    {
        internal readonly short x;
        internal readonly short y;
        internal readonly short z;

        private long uniqueCode;

        internal const int Max = short.MaxValue;
        internal const int Min = short.MinValue;

        /// <summary>
        /// シリアライズ用なのでそれ以外では使用しないでください
        /// </summary>
        internal ChunkCoordinate(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            uniqueCode = CalcUniqueCode(x, y, z);
        }

        /// <param name="ignoreValidation">パフォーマンス追及以外の用途では絶対に使用しないでください</param>
        internal ChunkCoordinate(int x, int y, int z, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
            {
                if (!IsValid(x, y, z)) throw new System.Exception($"chunk({x}, {y}, {z}) is invalid");
            }

            this.x = (short)x;
            this.y = (short)y;
            this.z = (short)z;

            uniqueCode = CalcUniqueCode(x, y, z);
        }

        private static long CalcUniqueCode(long x, long y, long z)
        {
            return (x << 32) + (y << 16) + z;
        }

        internal static bool IsValid(int x, int y, int z)
        {
            if (x < Min || x > Max) return false;
            if (y < Min || y > Max) return false;
            if (z < Min || z > Max) return false;
            return true;
        }

        internal static ChunkCoordinate FromBlockCoordinate(BlockCoordinate bc)
        {
            return new ChunkCoordinate(
                bc.x >> ChunkData.ChunkBlockSideShift,
                bc.y >> ChunkData.ChunkBlockSideShift,
                bc.z >> ChunkData.ChunkBlockSideShift
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
            return this.uniqueCode == other.uniqueCode;
        }

        public override int GetHashCode()
        {
            // TODO yとzしか使えない
            return (int)uniqueCode;
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
