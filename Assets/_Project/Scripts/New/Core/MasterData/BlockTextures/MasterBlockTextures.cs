using System;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using Zenject;

namespace MasterData
{
    [CreateAssetMenu(fileName = "MasterBlockTextures", menuName = "ScriptableObjects/MasterBlockTextures")]
    public class MasterBlockTextures : ScriptableObject, IDisposable
    {
        [SerializeField] private List<Asset_3D_Blocks_3_0_Texture> asset_3D_Blocks_3_0_Textures;
        [SerializeField] private List<SixTexture> sixTextures;

        public IReadOnlyDictionary<BlockType, BlockTexture> BlockTextures => _blockTextures ??= SetUpBlockTextures(asset_3D_Blocks_3_0_Textures, sixTextures);
        private Dictionary<BlockType, BlockTexture> _blockTextures;

        private static Dictionary<BlockType, BlockTexture> SetUpBlockTextures(params IEnumerable<IBlockTextureConvertible>[] collections)
        {
            var blockTextures = new Dictionary<BlockType, BlockTexture>();

            foreach (var collection in collections)
            {
                foreach (var item in collection)
                {
                    var blockTexture = item.ToBlockTexture();
                    blockTextures.Add(blockTexture.blockType, blockTexture);
                }
            }

            return blockTextures;
        }

        public void Dispose()
        {
            foreach (var texture in asset_3D_Blocks_3_0_Textures)
            {
                texture.Dispose();
            }
        }
    }
}
