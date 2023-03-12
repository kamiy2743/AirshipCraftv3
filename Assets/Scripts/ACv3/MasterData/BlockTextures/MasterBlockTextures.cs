using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACv3.MasterData
{
    [CreateAssetMenu(fileName = "MasterBlockTextures", menuName = "ScriptableObjects/MasterBlockTextures")]
   public class MasterBlockTextures : ScriptableObject, IDisposable
    {
        [SerializeField] List<Asset_3D_Blocks_3_0_Texture> _asset_3D_Blocks_3_0_Textures;
        internal IReadOnlyList<Asset_3D_Blocks_3_0_Texture> Asset_3D_Blocks_3_0_Textures => _asset_3D_Blocks_3_0_Textures;

        public void Dispose()
        {
            foreach (var texture in _asset_3D_Blocks_3_0_Textures)
            {
                texture.Dispose();
            }
        }
    }
}
