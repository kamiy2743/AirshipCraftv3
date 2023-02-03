using System;
using Unity.Mathematics;

namespace Domain.Chunks
{
    public record RelativeCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public const int Max = Chunk.BlockSide - 1;
        public const int Min = 0;

        public RelativeCoordinate(int x, int y, int z)
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

        public RelativeCoordinate Add(int3 value)
        {
            return Add(value.x, value.y, value.z);
        }

        internal RelativeCoordinate Add(int x = 0, int y = 0, int z = 0)
        {
            return new RelativeCoordinate(
                (this.x + x) & Max,
                (this.y + y) & Max,
                (this.z + z) & Max
            );
        }

        public static RelativeCoordinate Parse(BlockGridCoordinate blockGridCoordinate)
        {
            return new RelativeCoordinate(
                blockGridCoordinate.x & Max,
                blockGridCoordinate.y & Max,
                blockGridCoordinate.z & Max
            );
        }
    }
}