using UnityEngine;
using Domain;

namespace RenderingOptimization
{
    internal class BlockMeshProvider : IBlockMeshProvider
    {
        public BlockMesh GetBlockMesh(BlockTypeID blockTypeID)
        {
            if (blockTypeID == BlockTypeID.Air)
            {
                return new BlockMesh(new Vector3[0], new int[0], new Vector2[0]);
            }

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cubeMesh = cube.GetComponent<MeshFilter>().mesh;
            MonoBehaviour.Destroy(cube);
            return new BlockMesh(cubeMesh.vertices, cubeMesh.triangles, cubeMesh.uv);
        }
    }
}