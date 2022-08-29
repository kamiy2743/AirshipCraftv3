using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    public class ChunkData
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }

        public ChunkData(ChunkCoordinate cc)
        {
            ChunkCoordinate = cc;
        }
    }
}
