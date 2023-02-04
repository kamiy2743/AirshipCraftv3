using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    internal class BlockMeshProvider
    {
        private BlockMeshFactory blockMeshFactory;
        private IBlockShapeMeshProvider blockShapeMeshProvider;

        private BlockMesh air;
        private Dictionary<BlockType, BlockMesh> blockMeshes = new Dictionary<BlockType, BlockMesh>();

        internal BlockMeshProvider(BlockMeshFactory blockMeshFactory, IBlockShapeMeshProvider blockShapeMeshProvider)
        {
            this.blockMeshFactory = blockMeshFactory;
            this.blockShapeMeshProvider = blockShapeMeshProvider;

            air = blockMeshFactory.Create(BlockType.Air, new Vector3[0], new int[0], new Vector2[0]);

            foreach (var blockType in BlockTypeExt.Array)
            {
                var cubeMesh = blockShapeMeshProvider.GetMesh(BlockShape.Cube);
                var blockMesh = blockMeshFactory.Create(blockType, cubeMesh.vertices, cubeMesh.triangles, cubeMesh.uv);
                blockMeshes.Add(blockType, blockMesh);
            }
        }

        internal BlockMesh GetBlockMesh(BlockType blockType)
        {
            if (blockType == BlockType.Air)
            {
                return air;
            }

            return blockMeshes[blockType];
        }
    }
}