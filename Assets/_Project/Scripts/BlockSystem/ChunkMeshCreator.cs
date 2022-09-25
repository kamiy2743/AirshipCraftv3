using MasterData.Block;
using System.Collections.Generic;
using Util;
using UnityEngine;

namespace BlockSystem
{
    internal class ChunkMeshCreator
    {
        private ContactOtherBlockSolver _contactOtherBlockSolver;
        private ChunkDataStore _chunkDataStore;

        internal ChunkMeshCreator(ContactOtherBlockSolver contactOtherBlockSolver, ChunkDataStore chunkDataStore)
        {
            _contactOtherBlockSolver = contactOtherBlockSolver;
            _chunkDataStore = chunkDataStore;
        }

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// </summary>
        /// <param name="meshData">使いまわし用のChunkMeshData</param>
        /// <returns></returns>
        internal ChunkMeshData CreateMeshData(ChunkData chunkData, ChunkMeshData meshData = null)
        {
            var chunkDataCache = new Dictionary<ChunkCoordinate, ChunkData>(7);
            chunkDataCache.Add(chunkData.ChunkCoordinate, chunkData);
            foreach (var surface in SurfaceNormalExt.List)
            {
                var surfaceVector = surface.ToVector3Int();
                var ccx = chunkData.ChunkCoordinate.x + surfaceVector.x;
                var ccy = chunkData.ChunkCoordinate.y + surfaceVector.y;
                var ccz = chunkData.ChunkCoordinate.z + surfaceVector.z;
                if (!ChunkCoordinate.IsValid(ccx, ccy, ccz)) continue;

                var cc = new ChunkCoordinate(ccx, ccy, ccz);
                chunkDataCache.Add(cc, _chunkDataStore.GetChunkData(cc));
            }

            for (int i = 0; i < chunkData.Blocks.Length; i++)
            {
                if (chunkData.Blocks[i].NeedToCalcContactSurfaces)
                {
                    // 他のブロックに接している面を計算
                    var contactOtherBlockSurfaces = _contactOtherBlockSolver.GetContactOtherBlockSurfaces(chunkData.Blocks[i].BlockCoordinate, chunkDataCache);
                    chunkData.Blocks[i].SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);
                }

                if (meshData != null)
                {
                    meshData.AddBlock(chunkData.Blocks[i]);
                }
            }

            if (meshData != null) return meshData;

            int maxVerticesCount = 0;
            int maxTrianglesCount = 0;
            int maxUVsCount = 0;
            foreach (var block in chunkData.Blocks)
            {
                if (block.IsRenderSkip) continue;

                var blockMeshData = MasterBlockDataStore.GetData(block.ID).MeshData;
                maxVerticesCount += blockMeshData.Vertices.Length;
                maxTrianglesCount += blockMeshData.Triangles.Length;
                maxUVsCount += blockMeshData.UVs.Length;
            }

            meshData = new ChunkMeshData(maxVerticesCount, maxTrianglesCount, maxUVsCount);

            foreach (var blockData in chunkData.Blocks)
            {
                meshData.AddBlock(blockData);
            }

            return meshData;
        }
    }
}
