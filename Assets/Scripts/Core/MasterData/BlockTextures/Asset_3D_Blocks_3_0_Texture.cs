using System;
using System.Collections.Generic;
using ACv3.Domain;
using UnityEngine;
using ACv3.UnityView.Rendering;

namespace ACv3.MasterData
{
    [Serializable]
    record Asset_3D_Blocks_3_0_Texture : IDisposable
    {
        public BlockType blockType;
        public Texture2D texture;

        int size => texture.width / 2;
        List<Texture> createdTextures = new();

        public SixFaceTexture ToSixFaceTexture()
        {
            var rightLeftPixels = texture.GetPixels(0, 0, size, size);
            var forwardBackwardPixels = texture.GetPixels(size, 0, size, size);
            var topPixels = texture.GetPixels(size, size, size, size);
            var bottomPixels = texture.GetPixels(0, size, size, size);

            var rightLeftTexture = CreateTexture(rightLeftPixels);
            var forwardBackwardTexture = CreateTexture(forwardBackwardPixels);
            var topTexture = CreateTexture(topPixels);
            var bottomTexture = CreateTexture(bottomPixels);

            return new SixFaceTexture(
                rightLeftTexture,
                rightLeftTexture,
                topTexture,
                bottomTexture,
                forwardBackwardTexture,
                forwardBackwardTexture
            );
        }

        Texture2D CreateTexture(Color[] pixels)
        {
            var texture = new Texture2D(size, size);

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