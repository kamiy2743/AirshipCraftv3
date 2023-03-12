using System.Collections.Generic;
using ACv3.Domain;
using UnityEngine;

namespace ACv3.UnityView.Rendering
{
    public class BlockMeshProvider
    {
        readonly BlockMesh air;
        readonly Dictionary<BlockType, BlockMesh> blockMeshes = new();

        internal BlockMeshProvider(BlockMeshFactory blockMeshFactory, IBlockShapeMeshProvider blockShapeMeshProvider)
        {
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