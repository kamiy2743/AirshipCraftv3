using System;
using Domain.Chunks;
using Unity.Mathematics;

namespace Domain
{
    public record BlockGridCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        const int Max = ChunkGridCoordinate.Max * Chunk.BlockSide;
        const int Min = ChunkGridCoordinate.Min * Chunk.BlockSide;

        public BlockGridCoordinate(float3 value) : this((int)math.floor(value.x), (int)math.floor(value.y), (int)math.floor(value.z)) { }
        internal BlockGridCoordinate(int x, int y, int z)
        {
            if (!IsValid(x, y, z))
            {
                throw new ArgumentException($"{x}, {y}, {z}");
            }

            this.x = x;
            this.y = y;
            this.z = z;
        }

        bool IsValid(int x, int y, int z)
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
                result = new BlockGridCoordinate(x + value.x, y + value.y, z + value.z);
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
            return new float3(x, y, z);
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