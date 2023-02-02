using System.Linq;
using UnityEngine;
using Domain;
using MasterData;

namespace UnityView.Rendering
{
    internal class BlockMeshProvider
    {
        private MasterBlockShapeMeshes masterBlockShapeMeshes;

        private BlockMesh air;
        private BlockMesh other;

        internal BlockMeshProvider(MasterBlockShapeMeshes masterBlockShapeMeshes)
        {
            this.masterBlockShapeMeshes = masterBlockShapeMeshes;

            air = new BlockMesh(new Vector3[0], new int[0], new Vector2[0]);

            var cubeMesh = masterBlockShapeMeshes.BlockShapeMeshDic[BlockShapeID.Cube];
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