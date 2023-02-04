using System;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using UnityView.Rendering;

namespace MasterData
{
    [Serializable]
    internal record Asset_3D_Blocks_3_0_Texture : IDisposable
    {
        public BlockType blockType;
        public Texture2D texture;

        private int size => texture.width / 2;
        private List<Texture> createdTextures = new List<Texture>();

        public SixFaceTexture ToSixFaceTexture()
        {
            var rightLeftPixels = texture.GetPixels(0, 0, size, size);
            var forwardBackPixels = texture.GetPixels(size, 0, size, size);
            var topPixels = texture.GetPixels(size, size, size, size);
            var bottomPixels = texture.GetPixels(0, size, size, size);

            var rightLeftTexture = CreateTexture(rightLeftPixels);
            var forwardBackTexture = CreateTexture(forwardBackPixels);
            var topTexture = CreateTexture(topPixels);
            var bottomTexture = CreateTexture(bottomPixels);

            return new SixFaceTexture(
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