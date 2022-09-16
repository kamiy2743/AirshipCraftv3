using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterData.Block;

namespace BlockSystem
{
    public class SetUpBlockMaterial : MonoBehaviour
    {
        [SerializeField] private Material blockMaterial;
        private const int blockTextureSize = 128;

        public void StartInitial()
        {
            blockMaterial.mainTexture = GenerateTexture();
        }

        private Texture2D GenerateTexture()
        {
            var side = Mathf.CeilToInt(Mathf.Sqrt(BlockIDExt.MaxValue + 1));
            var texture = new Texture2D(blockTextureSize * side, blockTextureSize * side, TextureFormat.RGBA32, false);

            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    Texture2D blockTexture = null;

                    var masterBlockData = MasterBlockDataStore.GetData(side * y + x);
                    if (masterBlockData != null)
                    {
                        blockTexture = masterBlockData.Texture;
                    }
                    else
                    {
                        blockTexture = MasterBlockDataStore.GetData((int)BlockID.Empty).Texture;
                    }

                    texture.SetPixels(
                        x * blockTextureSize,
                        y * blockTextureSize,
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
