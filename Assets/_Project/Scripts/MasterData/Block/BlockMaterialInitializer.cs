using UnityEngine;

namespace MasterData.Block
{
    internal class BlockMaterialInitializer
    {
        internal BlockMaterialInitializer(Material blockMaterial, int blockCount)
        {
            blockMaterial.mainTexture = GenerateTexture(blockCount);
        }

        private Texture2D GenerateTexture(int blockCount)
        {
            var side = Mathf.CeilToInt(Mathf.Sqrt(blockCount));
            var blockTextureSize = MasterBlockDataStore.GetData(BlockID.Empty).Texture.width;
            var texture = new Texture2D((blockTextureSize + 2) * side, (blockTextureSize + 2) * side, TextureFormat.ARGB32, false);
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
                        x * (blockTextureSize + 2),
                        y * (blockTextureSize + 2),
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + 2),
                        y * (blockTextureSize + 2) + 2,
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + 2) + 2,
                        y * (blockTextureSize + 2) + 2,
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + 2) + 2,
                        y * (blockTextureSize + 2),
                        blockTextureSize,
                        blockTextureSize,
                        blockTexture.GetPixels());

                    texture.SetPixels(
                        x * (blockTextureSize + 2) + 1,
                        y * (blockTextureSize + 2) + 1,
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
