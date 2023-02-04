using System.Linq;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    internal class SixFaceUVCreator
    {
        internal Vector2[] Create(BlockType blockType, Vector2[] originalUVs)
        {
            var toAtlasUVScale = (float)SixFaceTexture.Size / (float)SixFaceTextureAtlasCreator.CalcTextureSize();

            // TODO SixFaceTextureAtlasCreatorと重複
            var side = SixFaceTexture.TextureCount * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, SixFaceTexture.TextureCount));
            var pivot = new Vector2((int)blockType / side * SixFaceTexture.TextureCount, (int)blockType % side) * toAtlasUVScale;

            return originalUVs
                .Select(uv =>
                {
                    var x = pivot.x + (uv.x * SixFaceTexture.TextureCount * toAtlasUVScale);
                    var y = pivot.y + (uv.y * toAtlasUVScale);
                    return new Vector2(x, y);
                })
                .ToArray();
        }
    }
}