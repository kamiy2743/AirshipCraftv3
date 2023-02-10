using System;
using Unity.Mathematics;
using Domain.Chunks;

namespace Domain
{
    public record BlockGridCoordinate
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        const int Max = ChunkGridCoordinate.Max * Chunk.BlockSide;
        const int Min = ChunkGridCoordinate.Min * Chunk.BlockSide;

        public BlockGridCoordinate(float3 value) : this((int)math.floor(value.x), (int)math.floor(value.y), (int)math.floor(value.z)) { }
        internal BlockGridCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z))
            {
                throw new ArgumentException($"{x}, {y}, {z}");
            }

            X = x;
            Y = y;
            Z = z;
        }

        static bool IsValid(int x, int y, int z)
        {
            if (x > Max || x < Min) return false;
            if (y > Max || y < Min) return false;
            if (z > Max || z < Min) return false;
            return true;
        }

        public bool TryAdd(int3 value, out BlockGridCoordinate result)
        {
            try
            {
                result = new BlockGridCoordinate(X + value.x, Y + value.y, Z + value.z);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public float3 ToPivotCoordinate()
        {
            return new float3(X, Y, Z);
        }

        public static bool TryParse(float3 value, out BlockGridCoordinate result)
        {
            try
            {
                result = new BlockGridCoordinate(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}