using System;
using UnityEngine;
using Domain;

namespace MasterData
{
    [Serializable]
    internal record SixTexture : IBlockTextureConvertible
    {
        public BlockType blockType;
        public Texture2D right;
        public Texture2D left;
        public Texture2D top;
        public Texture2D bottom;
        public Texture2D forward;
        public Texture2D back;

        public BlockTexture ToBlockTexture()
        {
            return new BlockTexture(blockType, right, left, top, bottom, forward, back);
        }
    }
}