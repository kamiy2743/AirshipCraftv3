using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSystem;
using MasterData.Block;

namespace BlockSystem
{
    public class ChunkObject : MonoBehaviour
    {
        public ChunkData ChunkData { get; private set; }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private int BlockSide => WorldSettings.LocalBlockSide;

        internal void Initial(ChunkData chunkData)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            ChunkData = chunkData;

            var meshCombiner = new MeshCombiner();

            // メッシュをバッチングする
            for (int y = 0; y < BlockSide; y++)
            {
                for (int z = 0; z < BlockSide; z++)
                {
                    for (int x = 0; x < BlockSide; x++)
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

            meshFilter.mesh = meshCombiner.Combine();
        }
    }
}
