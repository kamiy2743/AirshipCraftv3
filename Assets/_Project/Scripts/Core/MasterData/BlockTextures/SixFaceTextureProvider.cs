using Domain;
using UnityView.Rendering;

namespace MasterData
{
    internal class SixFaceTextureProvider : ISixFaceTextureProvider
    {
        private MasterBlockTextures masterBlockTextures;

        internal SixFaceTextureProvider(MasterBlockTextures masterBlockTextures)
        {
            this.masterBlockTextures = masterBlockTextures;
        }

        public bool TryGetSixFaceTexture(BlockType blockType, out SixFaceTexture result)
        {
            if (!masterBlockTextures.BlockTextures.TryGetValue(blockType, out var blockTexture))
            {
                result = null;
                return false;
            }

            result = new SixFaceTexture(
                blockTexture.right,
                blockTexture.left,
                blockTexture.top,
                blockTexture.bottom,
                blockTexture.forward,
                blockTexture.back);
            return true;
        }
    }
}