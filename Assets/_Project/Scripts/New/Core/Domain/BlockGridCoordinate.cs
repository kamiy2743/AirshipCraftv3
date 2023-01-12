using System;
using Unity.Mathematics;
using Domain.Chunks;

namespace Domain
{
    public record BlockGridCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        private const int Max = ChunkGridCoordinate.Max * Chunk.BlockSide;
        private const int Min = ChunkGridCoordinate.Min * Chunk.BlockSide;

        private BlockGridCoordinate(float3 value) : this((int)math.floor(value.x), (int)math.floor(value.y), (int)math.floor(value.z)) { }
        private BlockGridCoordinate(int3 value) : this(value.x, value.y, value.z) { }
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

        private bool IsValid(int x, int y, int z)
        {
            if (x > Max || x < Min) return false;
            if (y > Max || y < Min) return false;
            if (z > Max || z < Min) return false;
            return true;
        }

        internal static bool TryGetAdjacentCoordinate(AdjacentSurface surface, BlockGridCoordinate from, out BlockGridCoordinate result)
        {
            try
            {
                result = new BlockGridCoordinate(new int3(from.x, from.y, from.z) + surface.ToInt3());
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
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