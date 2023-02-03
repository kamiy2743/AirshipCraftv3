using System;
using UnityEngine;
using Zenject;
using MasterData;
using Domain;

namespace UnityView
{
    internal class BlockTextureAtlasCreator : IInitializable, IDisposable
    {
        internal Material blockMaterial;
        internal MasterBlockTextures masterBlockTextures;

        internal Texture2D createdTexture;

        internal BlockTextureAtlasCreator(Material blockMaterial, MasterBlockTextures masterBlockTextures)
        {
            this.blockMaterial = blockMaterial;
            this.masterBlockTextures = masterBlockTextures;
        }

        public void Initialize()
        {
            var textureSize = CalcTextureSize();
            var texture = new Texture2D(textureSize, textureSize);
            createdTexture = texture;

            foreach (var blockTexture in masterBlockTextures.BlockTextures.Values)
            {
                var pivot = CalcTexturePivot(blockTexture.blockType);
                var size = BlockTexture.Size;

                texture.SetPixels(pivot.x + size * 0, pivot.y, size, size, blockTexture.right.GetPixels());
                texture.SetPixels(pivot.x + size * 1, pivot.y, size, size, blockTexture.left.GetPixels());
                texture.SetPixels(pivot.x + size * 2, pivot.y, size, size, blockTexture.top.GetPixels());
                texture.SetPixels(pivot.x + size * 3, pivot.y, size, size, blockTexture.bottom.GetPixels());
                texture.SetPixels(pivot.x + size * 4, pivot.y, size, size, blockTexture.forward.GetPixels());
                texture.SetPixels(pivot.x + size * 5, pivot.y, size, size, blockTexture.back.GetPixels());
            }
            texture.Apply();

            blockMaterial.mainTexture = texture;
        }

        // ブロックのテクスチャを整列して並べられて、かつ2にべき乗のサイズを計算する
        // TODO まともな実装ではない
        internal static int CalcTextureSize()
        {
            var x = Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, BlockTexture.Count));
            var y = Mathf.CeilToInt(Mathf.Log(BlockTexture.Size * BlockTexture.Count * x, 2));
            var textureSize = (int)Mathf.Pow(2, y);

            return textureSize;
        }

        private Vector2Int CalcTexturePivot(BlockType blockType)
        {
            var side = BlockTexture.Count * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, BlockTexture.Count));
            return new Vector2Int((int)blockType / side * BlockTexture.Count, (int)blockType % side) * BlockTexture.Size;
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(createdTexture);
        }
    }
}
