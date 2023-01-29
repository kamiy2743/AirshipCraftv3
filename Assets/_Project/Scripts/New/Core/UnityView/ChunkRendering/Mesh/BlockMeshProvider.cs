using UnityEngine;
using Domain;

namespace UnityView.ChunkRendering.Mesh
{
    internal class BlockMeshProvider : IBlockMeshProvider
    {
        private BlockMesh air;
        private BlockMesh other;

        internal BlockMeshProvider()
        {
            air = new BlockMesh(new Vector3[0], new int[0], new Vector2[0]);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cubeMesh = cube.GetComponent<MeshFilter>().mesh;
            MonoBehaviour.Destroy(cube);
            other = new BlockMesh(cubeMesh.vertices, cubeMesh.triangles, cubeMesh.uv);
        }

        public BlockMesh GetBlockMesh(BlockTypeID blockTypeID)
        {
            if (blockTypeID == BlockTypeID.Air)
            {
                return air;
            }

            return other;
        }
    }
}