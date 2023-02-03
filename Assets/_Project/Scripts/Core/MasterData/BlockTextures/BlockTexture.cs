using UnityEngine;
using Domain;

namespace MasterData
{
    public record BlockTexture
    {
        public BlockType blockType;
        public Texture2D right;
        public Texture2D left;
        public Texture2D top;
        public Texture2D bottom;
        public Texture2D forward;
        public Texture2D back;

        public const int Size = 256;
        public const int Count = 6;

        internal BlockTexture(
            BlockType blockType,
            Texture2D right,
            Texture2D left,
            Texture2D top,
            Texture2D bottom,
            Texture2D forward,
            Texture2D back)
        {
            this.blockType = blockType;
            this.right = right;
            this.left = left;
            this.top = top;
            this.bottom = bottom;
            this.forward = forward;
            this.back = back;
        }
    }
}