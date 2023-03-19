using System.Collections.Generic;
using ACv3.Domain;
using ACv3.Presentation.Rendering;

namespace ACv3.MasterData
{
    public class SixFaceTextureProvider : ISixFaceTextureProvider
    {
        readonly Dictionary<BlockType, SixFaceTexture> textures = new();

        internal SixFaceTextureProvider(MasterBlockTextures masterBlockTextures)
        {
            foreach (var item in masterBlockTextures.Asset_3D_Blocks_3_0_Textures)
            {
                textures.Add(item.blockType, item.ToSixFaceTexture());
            }
        }

        public bool TryGetSixFaceTexture(BlockType blockType, out SixFaceTexture result)
        {
            return textures.TryGetValue(blockType, out result);
        }
    }
}