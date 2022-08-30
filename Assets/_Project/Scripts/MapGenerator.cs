using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    internal class MapGenerator
    {
        public static MapGenerator Instance => _instance;
        private static MapGenerator _instance = new MapGenerator();

        private float seedX;
        private float seedZ;
        private float relief = 80;

        public MapGenerator()
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
