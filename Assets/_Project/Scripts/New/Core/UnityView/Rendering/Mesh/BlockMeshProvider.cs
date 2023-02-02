using System.Linq;
using UnityEngine;
using Domain;
using MasterData;

namespace UnityView.Rendering
{
    internal class BlockMeshProvider
    {
        private BlockMeshFactory blockMeshFactory;
        private MasterBlockShapeMeshes masterBlockShapeMeshes;

        private BlockMesh air;
        private BlockMesh other;

        internal BlockMeshProvider(BlockMeshFactory blockMeshFactory, MasterBlockShapeMeshes masterBlockShapeMeshes)
        {
            this.blockMeshFactory = blockMeshFactory;
            this.masterBlockShapeMeshes = masterBlockShapeMeshes;

            air = blockMeshFactory.Create(new Vector3[0], new int[0], new Vector2[0]);

            var cubeMesh = masterBlockShapeMeshes.BlockShapeMeshes[BlockShape.Cube];
            var vertices = cubeMesh.vertices.Select(v => v + new Vector3(0.5f, 0.5f, 0.5f)).ToArray();
            other = blockMeshFactory.Create(vertices, cubeMesh.triangles, cubeMesh.uv);
        }

        internal BlockMesh GetBlockMesh(BlockType blockType)
        {
            if (blockType == BlockType.Air)
            {
                return air;
            }

            return other;
        }
    }
}