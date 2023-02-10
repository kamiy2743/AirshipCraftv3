using System;
using Unity.Mathematics;

namespace Domain.Chunks
{
    public record RelativeCoordinate
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public const int Max = Chunk.BlockSide - 1;
        public const int Min = 0;

        public RelativeCoordinate(int x, int y, int z)
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

        public RelativeCoordinate Add(int3 value)
        {
            return Add(value.x, value.y, value.z);
        }

        RelativeCoordinate Add(int x = 0, int y = 0, int z = 0)
        {
            return new RelativeCoordinate(
                (X + x) & Max,
                (Y + y) & Max,
                (Z + z) & Max
            );
        }

        public static RelativeCoordinate Parse(BlockGridCoordinate blockGridCoordinate)
        {
            return new RelativeCoordinate(
                blockGridCoordinate.X & Max,
                blockGridCoordinate.Y & Max,
                blockGridCoordinate.Z & Max
            );
        }
    }
}