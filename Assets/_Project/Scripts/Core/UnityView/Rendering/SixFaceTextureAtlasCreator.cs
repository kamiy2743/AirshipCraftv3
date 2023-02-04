using System;
using UnityEngine;
using Zenject;
using Domain;

namespace UnityView.Rendering
{
    internal class SixFaceTextureAtlasCreator : IInitializable, IDisposable
    {
        internal Material blockMaterial;
        internal ISixFaceTextureProvider sixFaceTextureProvider;

        internal Texture2D createdTexture;

        internal SixFaceTextureAtlasCreator(Material blockMaterial, ISixFaceTextureProvider sixFaceTextureProvider)
        {
            this.blockMaterial = blockMaterial;
            this.sixFaceTextureProvider = sixFaceTextureProvider;
        }

        public void Initialize()
        {
            var textureSize = CalcTextureSize();
            var texture = new Texture2D(textureSize, textureSize);
            createdTexture = texture;

            foreach (var blockType in BlockTypeExt.Array)
            {
                if (!sixFaceTextureProvider.TryGetSixFaceTexture(blockType, out var sixFaceTexture))
                {
                    continue;
                }

                var pivot = CalcTexturePivot(blockType);
                var size = SixFaceTexture.Size;

                texture.SetPixels(pivot.x + size * 0, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Right).GetPixels());
                texture.SetPixels(pivot.x + size * 1, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Left).GetPixels());
                texture.SetPixels(pivot.x + size * 2, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Top).GetPixels());
                texture.SetPixels(pivot.x + size * 3, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Bottom).GetPixels());
                texture.SetPixels(pivot.x + size * 4, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Forward).GetPixels());
                texture.SetPixels(pivot.x + size * 5, pivot.y, size, size, sixFaceTexture.GetFace(Direction.Back).GetPixels());
            }
            texture.Apply();

            blockMaterial.mainTexture = texture;
        }

        // ブロックのテクスチャを整列して並べられて、かつ2にべき乗のサイズを計算する
        internal static int CalcTextureSize()
        {
            var x = Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            var y = Mathf.CeilToInt(Mathf.Log(SixFaceTexture.Size * SixFaceTexture.TextureCount * x, 2));
            var textureSize = (int)Mathf.Pow(2, y);

            return textureSize;
        }

        private Vector2Int CalcTexturePivot(BlockType blockType)
        {
            var side = SixFaceTexture.TextureCount * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            return new Vector2Int((int)blockType / side * SixFaceTexture.TextureCount, (int)blockType % side) * SixFaceTexture.Size;
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(createdTexture);
        }
    }
}
