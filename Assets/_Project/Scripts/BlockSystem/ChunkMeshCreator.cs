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

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// </summary>
        /// <param name="meshData">使いまわし用のChunkMeshData</param>
        /// <returns></returns>
        internal ChunkMeshData CreateMeshData(ref BlockData[] blocksInChunk, ChunkMeshData meshData = null)
        {
            for (int i = 0; i < blocksInChunk.Length; i++)
            {
                if (blocksInChunk[i].NeedToCalcContactSurfaces)
                {
                    // 他のブロックに接している面を計算
                    var contactOtherBlockSurfaces = _contactOtherBlockSolver.GetContactOtherBlockSurfaces(blocksInChunk[i].BlockCoordinate);
                    blocksInChunk[i].SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);
                }

                if (meshData != null)
                {
                    meshData.AddBlock(blocksInChunk[i]);
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
