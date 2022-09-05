using UnityEngine;

namespace BlockSystem
{
    public class ChunkMeshCreator
    {
        private ContactOtherBlockSolver _contactOtherBlockSolver;

        public ChunkMeshCreator(ContactOtherBlockSolver contactOtherBlockSolver)
        {
            _contactOtherBlockSolver = contactOtherBlockSolver;
        }

        public MeshData CreateMesh(ChunkData chunkData)
        {
            var meshData = new MeshData();

            // メッシュをバッチングする
            for (int y = 0; y < World.ChunkBlockSide; y++)
            {
                for (int z = 0; z < World.ChunkBlockSide; z++)
                {
                    for (int x = 0; x < World.ChunkBlockSide; x++)
                    {
                        var blockData = chunkData.GetBlockData(new LocalCoordinate(x, y, z));

                        // 他のブロックに接している面を計算
                        var contactOtherBlockSurfaces = _contactOtherBlockSolver.GetContactOtherBlockSurfaces(blockData.BlockCoordinate);
                        blockData.SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);

                        // 空気に接していない = 見えないので描画しない
                        if (!blockData.IsContactAir) continue;

                        meshData.AddBlock(blockData);
                    }
                }
            }

            return meshData;
        }
    }
}
