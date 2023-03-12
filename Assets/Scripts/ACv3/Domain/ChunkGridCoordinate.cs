using System;
using ACv3.Domain.Chunks;
using Unity.Mathematics;

namespace ACv3.Domain
{
    public record ChunkGridCoordinate
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        // TODO ワールド端のチャンクを実装する
        internal const int Max = short.MaxValue - 1;
        internal const int Min = short.MinValue + 1;

        internal ChunkGridCoordinate(int x, int y, int z)
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

        public static bool TryConstruct(int x, int y, int z, out ChunkGridCoordinate result)
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
            return TryConstruct(x + value.x, y + value.y, z + value.z, out result);
        }

        public float3 ToPivotCoordinate()
        {
            return new float3(x, y, z) * Chunk.BlockSide;
        }

        public static ChunkGridCoordinate Parse(BlockGridCoordinate blockGridCoordinate)
        {
            return new ChunkGridCoordinate(
                (short)(blockGridCoordinate.x >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.y >> Chunk.BlockSideShift),
                (short)(blockGridCoordinate.z >> Chunk.BlockSideShift)
            );
        }
    }
}