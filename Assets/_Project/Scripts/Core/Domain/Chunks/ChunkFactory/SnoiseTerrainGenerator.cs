using Unity.Mathematics;

namespace Domain.Chunks
{
    readonly struct SnoiseTerrainGenerator
    {
        readonly float _seedX;
        readonly float _seedY;
        readonly float _seedZ;
        readonly float _relief;

        static float Snoise(float x, float z) => (noise.snoise(new float2(x, z)) + 1) * 0.5f;
        static float Snoise(float x, float y, float z) => (noise.snoise(new float3(x, y, z)) + 1) * 0.5f;

        internal SnoiseTerrainGenerator(uint seed, float relief)
        {
            var random = new Random(seed);
            _seedX = random.NextFloat(0, 1) * seed;
            _seedY = random.NextFloat(0, 1) * seed;
            _seedZ = random.NextFloat(0, 1) * seed;
            _relief = relief;
        }

        internal BlockType GetBlockType(int x, int y, int z)
        {
            if (y >= 0) return ForGround(x, y, z);
            return ForUnderGround(x, y, z);
        }

        BlockType ForGround(int x, int y, int z)
        {
            var noise1 = Snoise(
                (x + _seedX) * _relief,
                (z + _seedZ) * _relief);

            var noiseY = (int)(noise1 * 16);

            if (noiseY == y) return BlockType.Grass;
            if (noiseY > y) return BlockType.Dirt;
            return BlockType.Air;
        }

        BlockType ForUnderGround(int x, int y, int z)
        {
            var noise = Snoise(
                (x + _seedX) * _relief,
                (y + _seedY) * _relief,
                (z + _seedZ) * _relief);

            if (noise > 0.4f) return BlockType.Stone;
            return BlockType.Air;
        }
    }
}