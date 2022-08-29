using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        public ChunkData ChunkData { get; private set; }

        private int BlockSide => WorldSettings.LocalBlockSide;

        internal void Initial(ChunkData chunkData)
        {
            ChunkData = chunkData;
            var cc = chunkData.ChunkCoordinate;
            transform.position = new Vector3(cc.x, cc.y, cc.z) * BlockSide;

            for (int y = 0; y < BlockSide; y++)
            {
                for (int z = 0; z < BlockSide; z++)
                {
                    for (int x = 0; x < BlockSide; x++)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        go.SetParent(transform);
                        go.position = transform.position + new Vector3(x, y, z) + Vector3.one * 0.5f;
                    }
                }
            }
        }
    }
}
