using System;
using UnityEngine;
using Domain;

namespace MasterData
{
    [Serializable]
    internal record Asset_3D_Blocks_3_0_Texture : IBlockTextureConvertible
    {
        public BlockType blockType;
        public Texture2D texture;

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

        // TODO textureは明示的に廃棄しないといけないかも
        private Texture2D CreateTexture(Color[] pixels)
        {
            var result = new Texture2D(UnitSize, UnitSize);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
    }
}