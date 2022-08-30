using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        public ChunkData ChunkData { get; private set; }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private int BlockSide => WorldSettings.LocalBlockSide;

        internal void Initial(ChunkData chunkData)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            ChunkData = chunkData;
            var cc = chunkData.ChunkCoordinate;
            transform.position = new Vector3(cc.x, cc.y, cc.z) * BlockSide;

            var meshCombiner = new MeshCombiner();
            // メッシュをバッチングする
            for (int y = 0; y < BlockSide; y++)
            {
                for (int z = 0; z < BlockSide; z++)
                {
                    for (int x = 0; x < BlockSide; x++)
                    {
                        meshCombiner.AddMeshData(CubeMesh.Vertices, CubeMesh.Triangles, new Vector3(x, y, z));
                    }
                }
            }

            meshFilter.mesh = meshCombiner.Combine();
        }
    }
}
