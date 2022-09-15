using UnityEngine;
using System.Collections.Generic;
using MasterData.Block;

namespace BlockSystem
{
    public class ChunkMeshCreator
    {
        private ContactOtherBlockSolver _contactOtherBlockSolver;

        public ChunkMeshCreator(ContactOtherBlockSolver contactOtherBlockSolver)
        {
            _contactOtherBlockSolver = contactOtherBlockSolver;
        }

        public ChunkMeshData CreateMeshData(IReadOnlyCollection<BlockData> blocksInChunk, ChunkMeshData meshData = null)
        {
            if (meshData == null)
            {
                int maxVerticesCount = 0;
                int maxTrianglesCount = 0;
                int maxUVsCount = 0;

                foreach (var block in blocksInChunk)
                {
                    var blockMeshData = MasterBlockDataStore.GetData(block.ID.value).MeshData;
                    maxVerticesCount += blockMeshData.Vertices.Length;
                    maxTrianglesCount += blockMeshData.Triangles.Length;
                    maxUVsCount += blockMeshData.UVs.Length;
                }

                meshData = new ChunkMeshData(maxVerticesCount, maxTrianglesCount, maxUVsCount);
            }

            foreach (var blockData in blocksInChunk)
            {
                // 他のブロックに接している面を計算
                var contactOtherBlockSurfaces = _contactOtherBlockSolver.GetContactOtherBlockSurfaces(blockData.BlockCoordinate);
                blockData.SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);

                // 空気に接していない = 見えないので描画しない
                if (!blockData.IsContactAir) continue;

                meshData.AddBlock(blockData);
            }

            return meshData;
        }
    }
}
