using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        public ChunkData ChunkData { get; private set; }

        internal void Initial(ChunkData chunkData)
        {
            ChunkData = chunkData;
            var cc = chunkData.ChunkCoordinate;
            transform.position = new Vector3(cc.x, cc.y, cc.z) * WorldSettings.LocalBlockSide;
        }
    }
}
