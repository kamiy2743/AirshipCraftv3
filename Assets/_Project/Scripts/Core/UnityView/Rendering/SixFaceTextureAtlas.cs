using System;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    record SixFaceTextureAtlas : IBlockTextureAtlas, IDisposable
    {
        readonly Texture2D _texture;
        internal int Size => _texture.width;

        internal SixFaceTextureAtlas(ISixFaceTextureProvider sixFaceTextureProvider)
        {
            var textureSize = CalcTextureSize();
            _texture = new Texture2D(textureSize, textureSize);

            foreach (var blockType in BlockTypeExt.Array)
            {
                if (!sixFaceTextureProvider.TryGetSixFaceTexture(blockType, out var sixFaceTexture))
                {
                    continue;
                }

                var pivot = GetPivot(blockType);
                const int size = SixFaceTexture.Size;

                _texture.SetPixels(pivot.x + size * 0, pivot.y, size, size, sixFaceTexture.GetFace(Face.Right).GetPixels());
                _texture.SetPixels(pivot.x + size * 1, pivot.y, size, size, sixFaceTexture.GetFace(Face.Left).GetPixels());
                _texture.SetPixels(pivot.x + size * 2, pivot.y, size, size, sixFaceTexture.GetFace(Face.Top).GetPixels());
                _texture.SetPixels(pivot.x + size * 3, pivot.y, size, size, sixFaceTexture.GetFace(Face.Bottom).GetPixels());
                _texture.SetPixels(pivot.x + size * 4, pivot.y, size, size, sixFaceTexture.GetFace(Face.Front).GetPixels());
                _texture.SetPixels(pivot.x + size * 5, pivot.y, size, size, sixFaceTexture.GetFace(Face.Back).GetPixels());
            }
            _texture.Apply();
        }

        // ブロックのテクスチャを整列して並べられて、かつ2にべき乗のサイズを計算する
        static int CalcTextureSize()
        {
            var x = Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            var y = Mathf.CeilToInt(Mathf.Log(SixFaceTexture.Size * SixFaceTexture.TextureCount * x, 2));
            var textureSize = (int)Mathf.Pow(2, y);

            return textureSize;
        }

        public Texture2D GetAtlas()
        {
            return _texture;
        }

        internal Vector2Int GetPivot(BlockType blockType)
        {
            var side = SixFaceTexture.TextureCount * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            return new Vector2Int((int)blockType / side * SixFaceTexture.TextureCount, (int)blockType % side) * SixFaceTexture.Size;
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(_texture);
        }
    }
}