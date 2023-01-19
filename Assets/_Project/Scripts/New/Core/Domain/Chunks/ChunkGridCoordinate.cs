using System;
using Unity.Mathematics;

namespace Domain.Chunks
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

        private bool IsValid(int x, int y, int z)
        {
            if (x > Max || x < Min) return false;
            if (y > Max || y < Min) return false;
            if (z > Max || z < Min) return false;
            return true;
        }

        public bool TryAdd(int3 value, out ChunkGridCoordinate result)
        {
            try
            {
                result = new ChunkGridCoordinate((short)(this.x + value.x), (short)(this.y + value.y), (short)(this.z + value.z));
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
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