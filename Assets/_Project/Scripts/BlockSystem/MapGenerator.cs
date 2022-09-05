using UnityEngine;

namespace BlockSystem
{
    public class MapGenerator
    {
        private float seedX;
        private float seedZ;
        private float _relief;

        public MapGenerator(float relief)
        {
            seedX = Random.value * 100f;
            seedZ = Random.value * 100f;
            _relief = relief;
        }

        public int GetBlockID(BlockCoordinate bc)
        {
            float xSample = (bc.x + seedX) / _relief;
            float zSample = (bc.z + seedZ) / _relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);

            var y = World.WorldBlockSideY * noise;
            return (bc.y <= y ? 1 : 0);
        }
    }
}
