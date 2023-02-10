using System;
using Unity.Mathematics;
using Domain.Chunks;

namespace Domain
{
    public record ChunkGridCoordinate
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        // TODO ワールド端のチャンクを実装する
        internal const int Max = short.MaxValue - 1;
        internal const int Min = short.MinValue + 1;

        internal ChunkGridCoordinate(int x, int y, int z)
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

        static bool TryConstruct(int x, int y, int z, out ChunkGridCoordinate result)
        {
            try
            {
                result = new ChunkGridCoordinate(x, y, z);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public bool TryAdd(int3 value, out ChunkGridCoordinate result)
        {
            return TryConstruct(X + value.x, Y + value.y, Z + value.z, out result);
        }

        public float3 ToPivotCoordinate()
        {
            return new float3(X, Y, Z) * Chunk.BlockSide;
        }

        public static ChunkGridCoordinate Parse(BlockGridCoordinate blockGridCoordinate)
        {
            return new ChunkGridCoordinate(
                (short)(blockGridCoordinate.X >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.Y >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.Z >> Chunk.BlockSideShift)
            );
        }
    }
}