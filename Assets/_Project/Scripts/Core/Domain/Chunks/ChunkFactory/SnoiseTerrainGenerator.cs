using Unity.Mathematics;

namespace Domain.Chunks
{
    readonly struct SnoiseTerrainGenerator
    {
        readonly float seedX;
        readonly float seedY;
        readonly float seedZ;
        readonly float relief;

        static float Snoise(float x, float z) => (noise.snoise(new float2(x, z)) + 1) * 0.5f;
        static float Snoise(float x, float y, float z) => (noise.snoise(new float3(x, y, z)) + 1) * 0.5f;

        internal SnoiseTerrainGenerator(uint seed, float relief)
        {
            var random = new Random(seed);
            seedX = random.NextFloat(0, 1) * seed;
            seedY = random.NextFloat(0, 1) * seed;
            seedZ = random.NextFloat(0, 1) * seed;
            this.relief = relief;
        }

        internal BlockType GetBlockType(int x, int y, int z)
        {
            if (y >= 0) return ForGround(x, y, z);
            return ForUnderGround(x, y, z);
        }

        BlockType ForGround(int x, int y, int z)
        {
            var noise1 = Snoise(
                (x + seedX) * relief,
                (z + seedZ) * relief);

            var noiseY = (int)(noise1 * 16);

            if (noiseY == y) return BlockType.Grass;
            if (noiseY > y) return BlockType.Dirt;
            return BlockType.Air;
        }

        BlockType ForUnderGround(int x, int y, int z)
        {
            var noise = Snoise(
                (x + seedX) * relief,
                (y + seedY) * relief,
                (z + seedZ) * relief);

            if (noise > 0.4f) return BlockType.Stone;
            return BlockType.Air;
        }
    }
}