using System;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace MasterData
{
    [Serializable]
    internal record Asset_3D_Blocks_3_0_Texture : IBlockTextureConvertible, IDisposable
    {
        public BlockType blockType;
        public Texture2D texture;

        private List<Texture> createdTextures = new List<Texture>();

        private const int UnitSize = 256;

        public BlockTexture ToBlockTexture()
        {
            var rightLeftPixels = texture.GetPixels(0, 0, UnitSize, UnitSize);
            var forwardBackPixels = texture.GetPixels(UnitSize, 0, UnitSize, UnitSize);
            var topPixels = texture.GetPixels(UnitSize, UnitSize, UnitSize, UnitSize);
            var bottomPixels = texture.GetPixels(0, UnitSize, UnitSize, UnitSize);

            var rightLeftTexture = CreateTexture(rightLeftPixels);
            var forwardBackTexture = CreateTexture(forwardBackPixels);
            var topTexture = CreateTexture(topPixels);
            var bottomTexture = CreateTexture(bottomPixels);

            return new BlockTexture(
                blockType,
                rightLeftTexture,
                rightLeftTexture,
                topTexture,
                bottomTexture,
                forwardBackTexture,
                forwardBackTexture
            );
        }

        private Texture2D CreateTexture(Color[] pixels)
        {
            var texture = new Texture2D(UnitSize, UnitSize);

            texture.SetPixels(pixels);
            texture.Apply();
            createdTextures.Add(texture);

            return texture;
        }

        public void Dispose()
        {
            foreach (var texture in createdTextures)
            {
                MonoBehaviour.Destroy(texture);
            }
        }
    }
}