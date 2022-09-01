using UnityEngine;

namespace BlockSystem
{
    internal class MapGenerator
    {
        internal static MapGenerator Instance => _instance;
        private static MapGenerator _instance = new MapGenerator();

        private float seedX;
        private float seedZ;
        private float relief = 80;

        internal MapGenerator()
        {
            seedX = Random.value * 100f;
            seedZ = Random.value * 100f;
        }

        internal int GetBlockID(BlockCoordinate bc)
        {
            float xSample = (bc.x + seedX) / relief;
            float zSample = (bc.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);

            var y = WorldSettings.WorldBlockSideY * noise;
            return (bc.y <= y ? 1 : 0);
        }
    }
}
