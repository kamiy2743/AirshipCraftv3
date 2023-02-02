using UnityEngine;
using Domain;

namespace MasterData
{
    public record BlockTexture
    {
        internal BlockType blockType;
        public Texture2D right;
        public Texture2D left;
        public Texture2D top;
        public Texture2D bottom;
        public Texture2D forward;
        public Texture2D back;

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