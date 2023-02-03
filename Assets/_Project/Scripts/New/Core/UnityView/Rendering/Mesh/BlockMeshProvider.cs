using System.Collections.Generic;
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
        private Dictionary<BlockType, BlockMesh> blockMeshes = new Dictionary<BlockType, BlockMesh>();

        internal BlockMeshProvider(BlockMeshFactory blockMeshFactory, MasterBlockShapeMeshes masterBlockShapeMeshes)
        {
            this.blockMeshFactory = blockMeshFactory;
            this.masterBlockShapeMeshes = masterBlockShapeMeshes;

            air = blockMeshFactory.Create(BlockType.Air, new Vector3[0], new int[0], new Vector2[0]);

            foreach (var blockType in BlockTypeExt.Array)
            {
                var cubeMesh = masterBlockShapeMeshes.BlockShapeMeshes[BlockShape.Cube];
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