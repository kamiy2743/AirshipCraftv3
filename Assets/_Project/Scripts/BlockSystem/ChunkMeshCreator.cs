using UnityEngine;
using System.Collections.Generic;

namespace BlockSystem
{
    public class ChunkMeshCreator
    {
        private ContactOtherBlockSolver _contactOtherBlockSolver;

        public ChunkMeshCreator(ContactOtherBlockSolver contactOtherBlockSolver)
        {
            _contactOtherBlockSolver = contactOtherBlockSolver;
        }

        public MeshData CreateMeshData(IReadOnlyCollection<BlockData> blocksInChunk)
        {
            var meshData = new MeshData();

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
