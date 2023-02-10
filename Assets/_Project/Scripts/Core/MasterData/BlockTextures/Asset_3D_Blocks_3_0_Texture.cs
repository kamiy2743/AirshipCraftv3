using System;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using UnityView.Rendering;

namespace MasterData
{
    [Serializable]
    record Asset_3D_Blocks_3_0_Texture : IDisposable
    {
        public BlockType blockType;
        public Texture2D texture;

        int Size => texture.width / 2;
        List<Texture> _createdTextures = new List<Texture>();

        public SixFaceTexture ToSixFaceTexture()
        {
            var rightLeftPixels = texture.GetPixels(0, 0, Size, Size);
            var forwardBackwardPixels = texture.GetPixels(Size, 0, Size, Size);
            var topPixels = texture.GetPixels(Size, Size, Size, Size);
            var bottomPixels = texture.GetPixels(0, Size, Size, Size);

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
            var texture = new Texture2D(Size, Size);

            texture.SetPixels(pixels);
            texture.Apply();
            _createdTextures.Add(texture);

            return texture;
        }

        public void Dispose()
        {
            foreach (var texture in _createdTextures)
            {
                MonoBehaviour.Destroy(texture);
            }
        }
    }
}