using System.Collections.Generic;
using Domain;
using UnityView.Rendering;

namespace MasterData
{
    class SixFaceTextureProvider : ISixFaceTextureProvider
    {
        readonly Dictionary<BlockType, SixFaceTexture> _textures = new Dictionary<BlockType, SixFaceTexture>();

        internal SixFaceTextureProvider(MasterBlockTextures masterBlockTextures)
        {
            foreach (var item in masterBlockTextures.Asset_3D_Blocks_3_0_Textures)
            {
                _textures.Add(item.blockType, item.ToSixFaceTexture());
            }
        }

        public bool TryGetSixFaceTexture(BlockType blockType, out SixFaceTexture result)
        {
            return _textures.TryGetValue(blockType, out result);
        }
    }
}