using UnityEngine;

namespace BlockSystem
{
    public static class ChunkMeshCreator
    {
        public static Mesh CreateMesh(ChunkData chunkData)
        {
            var meshCombiner = new MeshCombiner();

            // メッシュをバッチングする
            for (int y = 0; y < WorldSettings.LocalBlockSide; y++)
            {
                for (int z = 0; z < WorldSettings.LocalBlockSide; z++)
                {
                    for (int x = 0; x < WorldSettings.LocalBlockSide; x++)
                    {
                        var blockData = chunkData.GetBlockData(new LocalCoordinate(x, y, z));

                        // 他のブロックに接している面を計算
                        var contactOtherBlockSurfaces = ContactOtherBlockSolver.Instance.GetContactOtherBlockSurfaces(blockData.BlockCoordinate);
                        blockData.SetContactOtherBlockSurfaces(contactOtherBlockSurfaces);

                        // 空気に接していない = 見えないので描画しない
                        if (!blockData.IsContactAir) continue;

                        meshCombiner.AddBlock(blockData);
                    }
                }
            }

            return meshCombiner.Combine();
        }
    }
}
