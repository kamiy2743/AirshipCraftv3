using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace BlockSystem
{
    public struct MapGenerator
    {
        private Random random;
        private float seedX;
        private float seedZ;
        private float inverseRelief;

        public MapGenerator(uint seed, float relief)
        {
            random = new Random(seed);
            seedX = random.NextFloat(0, 1) * 100f;
            seedZ = random.NextFloat(0, 1) * 100f;
            inverseRelief = 1 / relief;
        }

        public BlockID GetBlockID(int x, int y, int z)
        {
            float xSample = (x + seedX) * inverseRelief;
            float zSample = (z + seedZ) * inverseRelief;
            var noiseValue = noise.snoise(new float2(xSample, zSample));
            noiseValue = (noiseValue + 1) * 0.5f;

            var resultY = World.WorldBlockSideY * noiseValue;
            return new BlockID(resultY <= y ? 0 : 1);
        }
    }
}
