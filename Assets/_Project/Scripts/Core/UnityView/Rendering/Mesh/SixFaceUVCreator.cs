using System.Linq;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    internal class SixFaceUVCreator
    {
        private SixFaceTextureAtlas sixFaceTextureAtlas;

        internal SixFaceUVCreator(SixFaceTextureAtlas sixFaceTextureAtlas)
        {
            this.sixFaceTextureAtlas = sixFaceTextureAtlas;
        }

        internal Vector2[] Create(BlockType blockType, Vector2[] originalUVs)
        {
            var toUVScale = (float)SixFaceTexture.Size / (float)sixFaceTextureAtlas.Size;
            var pivot = (Vector2)sixFaceTextureAtlas.GetPivot(blockType) / (float)SixFaceTexture.Size * toUVScale;

            return originalUVs
                .Select(uv =>
                {
                    var x = pivot.x + (uv.x * SixFaceTexture.TextureCount * toUVScale);
                    var y = pivot.y + (uv.y * toUVScale);
                    return new Vector2(x, y);
                })
                .ToArray();
        }
    }
}