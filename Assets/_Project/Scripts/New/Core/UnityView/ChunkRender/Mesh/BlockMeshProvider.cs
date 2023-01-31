using System.Linq;
using UnityEngine;
using Domain;

namespace UnityView.ChunkRender
{
    internal class BlockMeshProvider
    {
        private BlockMesh air;
        private BlockMesh other;

        internal BlockMeshProvider()
        {
            air = new BlockMesh(new Vector3[0], new int[0], new Vector2[0]);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cubeMesh = cube.GetComponent<MeshFilter>().mesh;
            MonoBehaviour.Destroy(cube);

            var vertices = cubeMesh.vertices.Select(v => v + new Vector3(0.5f, 0.5f, 0.5f)).ToArray();
            other = new BlockMesh(vertices, cubeMesh.triangles, cubeMesh.uv);
        }

        internal BlockMesh GetBlockMesh(BlockTypeID blockTypeID)
        {
            if (blockTypeID == BlockTypeID.Air)
            {
                return air;
            }

            return other;
        }
    }
}