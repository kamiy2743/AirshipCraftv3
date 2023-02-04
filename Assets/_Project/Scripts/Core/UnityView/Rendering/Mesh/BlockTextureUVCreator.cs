using System.Linq;
using UnityEngine;
using Domain;
using MasterData;

namespace UnityView.Rendering
{
    internal class BlockTextureUVCreator
    {
        internal Vector2[] Create(BlockType blockType, Vector2[] originalUVs)
        {
            var toAtlasUVScale = (float)BlockTexture.Size / (float)BlockTextureAtlasCreator.CalcTextureSize();

            // TODO BlockTextureAtlasCreatorと重複
            var side = BlockTexture.Count * Mathf.CeilToInt(Mathf.Log(BlockTypeExt.Array.Length, BlockTexture.Count));
            var pivot = new Vector2((int)blockType / side * BlockTexture.Count, (int)blockType % side) * toAtlasUVScale;

            return originalUVs
                .Select(uv =>
                {
                    var x = pivot.x + (uv.x * BlockTexture.Count * toAtlasUVScale);
                    var y = pivot.y + (uv.y * toAtlasUVScale);
                    return new Vector2(x, y);
                })
                .ToArray();
        }
    }
}