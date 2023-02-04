using System;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    internal record SixFaceTextureAtlas : IBlockTextureAtlas, IDisposable
    {
        private Texture2D texture;
        internal int Size => texture.width;

        internal SixFaceTextureAtlas(ISixFaceTextureProvider sixFaceTextureProvider)
        {
            var textureSize = CalcTextureSize();
            texture = new Texture2D(textureSize, textureSize);

            foreach (var blockType in BlockTypeExt.Array)
            {
                if (!sixFaceTextureProvider.TryGetSixFaceTexture(blockType, out var sixFaceTexture))
                {
                    continue;
                }

                var pivot = GetPivot(blockType);
                var size = SixFaceTexture.Size;

                texture.SetPixels(pivot.x + size * 0, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Right).GetPixels());
                texture.SetPixels(pivot.x + size * 1, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Left).GetPixels());
                texture.SetPixels(pivot.x + size * 2, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Top).GetPixels());
                texture.SetPixels(pivot.x + size * 3, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Bottom).GetPixels());
                texture.SetPixels(pivot.x + size * 4, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Forward).GetPixels());
                texture.SetPixels(pivot.x + size * 5, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Back).GetPixels());
            }
            texture.Apply();
        }

        // ブロックのテクスチャを整列して並べられて、かつ2にべき乗のサイズを計算する
        private int CalcTextureSize()
        {
            var x = Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            var y = Mathf.CeilToInt(Mathf.Log(SixFaceTexture.Size * SixFaceTexture.TextureCount * x, 2));
            var textureSize = (int)Mathf.Pow(2, y);

            return textureSize;
        }

        public Texture2D GetAtlas()
        {
            return texture;
        }

        internal Vector2Int GetPivot(BlockType blockType)
        {
            var side = SixFaceTexture.TextureCount * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            return new Vector2Int((int)blockType / side * SixFaceTexture.TextureCount, (int)blockType % side) * SixFaceTexture.Size;
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(texture);
        }
    }
}