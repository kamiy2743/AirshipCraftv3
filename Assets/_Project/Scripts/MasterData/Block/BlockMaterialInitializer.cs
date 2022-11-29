using UnityEngine;
using DataObject.Block;

namespace MasterData.Block
{
    internal class BlockMaterialInitializer
    {
        private const int EdgeMargin = 1;

        internal BlockMaterialInitializer(Material blockMaterial, int blockCount)
        {
            blockMaterial.mainTexture = GenerateTexture(blockCount);
        }

        private Texture2D GenerateTexture(int blockCount)
        {
            var side = Mathf.CeilToInt(Mathf.Sqrt(blockCount));
            var blockTextureSize = MasterBlockDataStore.GetData(BlockID.Empty).Texture.width;
            var texture = new Texture2D((blockTextureSize + EdgeMargin * 2) * side, (blockTextureSize + EdgeMargin * 2) * side, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    Texture2D blockTexture = null;

                    var masterBlockData = MasterBlockDataStore.GetData(side * y + x);
                    if (masterBlockData is not null)
                    {
                        blockTexture = masterBlockData.Texture;
                    }
                    else
                    {
                        blockTexture = MasterBlockDataStore.GetData(BlockID.Empty).Texture;
                    }

                    texture.SetPixels(
                        x * (blockTextureSize + EdgeMargin * 2),
                        y * (blockTextureSize + EdgeMargin * 2),
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + EdgeMargin * 2),
                        y * (blockTextureSize + EdgeMargin * 2) + EdgeMargin * 2,
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + EdgeMargin * 2) + EdgeMargin * 2,
                        y * (blockTextureSize + EdgeMargin * 2) + EdgeMargin * 2,
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + EdgeMargin * 2) + EdgeMargin * 2,
                        y * (blockTextureSize + EdgeMargin * 2),
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + EdgeMargin * 2) + EdgeMargin,
                        y * (blockTextureSize + EdgeMargin * 2) + EdgeMargin,
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());
                }
            }

            texture.Apply();
            return texture;
        }
    }
}
