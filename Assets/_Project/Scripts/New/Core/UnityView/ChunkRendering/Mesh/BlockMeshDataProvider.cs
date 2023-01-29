using UnityEngine;
using Domain;

namespace UnityView.ChunkRendering.Mesh
{
    internal class BlockMeshDataProvider
    {
        private BlockMeshData air;
        private BlockMeshData other;

        internal BlockMeshDataProvider()
        {
            air = new BlockMeshData(new Vector3[0], new int[0], new Vector2[0]);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cubeMesh = cube.GetComponent<MeshFilter>().mesh;
            MonoBehaviour.Destroy(cube);
            other = new BlockMeshData(cubeMesh.vertices, cubeMesh.triangles, cubeMesh.uv);
        }

        internal BlockMeshData GetBlockMeshData(BlockTypeID blockTypeID)
        {
            if (blockTypeID == BlockTypeID.Air)
            {
                return air;
            }

            return other;
        }
    }
}