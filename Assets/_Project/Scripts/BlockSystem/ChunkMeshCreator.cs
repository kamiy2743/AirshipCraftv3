using UnityEngine;
using System.Collections.Generic;
using MasterData.Block;

namespace BlockSystem
{
    internal class ChunkMeshCreator
    {
        private ContactOtherBlockSolver _contactOtherBlockSolver;

        internal ChunkMeshCreator(ContactOtherBlockSolver contactOtherBlockSolver)
        {
            _contactOtherBlockSolver = contactOtherBlockSolver;
        }

        internal ChunkMeshData CreateMeshData(BlockData[] blocksInChunk, ChunkMeshData meshData = null)
        {
            foreach (var blockData in blocksInChunk)
            {
                if (blockData.NeedToCalcContactSurfaces)
                {
                    // 他のブロックに接している面を計算
                    var contactOtherBlockSurfaces = _contactOtherBlockSolver.GetContactOtherBlockSurfaces(blockData.BlockCoordinate);
                    // TODO 参照渡しじゃないからChunkDataの方には反映されない
                    blockData.SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);
                }

                if (meshData != null)
                {
                    meshData.AddBlock(blockData);
                }
            }

            if (meshData != null) return meshData;

            int maxVerticesCount = 0;
            int maxTrianglesCount = 0;
            int maxUVsCount = 0;
            foreach (var block in blocksInChunk)
            {
                if (block.IsRenderSkip) continue;

                var blockMeshData = MasterBlockDataStore.GetData(block.ID).MeshData;
                maxVerticesCount += blockMeshData.Vertices.Length;
                maxTrianglesCount += blockMeshData.Triangles.Length;
                maxUVsCount += blockMeshData.UVs.Length;
            }

            meshData = new ChunkMeshData(maxVerticesCount, maxTrianglesCount, maxUVsCount);

            foreach (var blockData in blocksInChunk)
            {
                meshData.AddBlock(blockData);
            }

            return meshData;
        }
    }
}
