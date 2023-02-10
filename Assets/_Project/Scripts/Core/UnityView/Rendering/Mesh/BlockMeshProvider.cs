using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    class BlockMeshProvider
    {
        readonly BlockMesh _air;
        readonly Dictionary<BlockType, BlockMesh> _blockMeshes = new Dictionary<BlockType, BlockMesh>();

        internal BlockMeshProvider(BlockMeshFactory blockMeshFactory, IBlockShapeMeshProvider blockShapeMeshProvider)
        {
            _air = blockMeshFactory.Create(BlockType.Air, new Vector3[0], new int[0], new Vector2[0]);

            foreach (var blockType in BlockTypeExt.Array)
            {
                var cubeMesh = blockShapeMeshProvider.GetMesh(BlockShape.Cube);
                var blockMesh = blockMeshFactory.Create(blockType, cubeMesh.vertices, cubeMesh.triangles, cubeMesh.uv);
                _blockMeshes.Add(blockType, blockMesh);
            }
        }

        internal BlockMesh GetBlockMesh(BlockType blockType)
        {
            if (blockType == BlockType.Air)
            {
                return _air;
            }

            return _blockMeshes[blockType];
        }
    }
}