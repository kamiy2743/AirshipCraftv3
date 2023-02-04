using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace UnityView.Rendering
{
    public record SixFaceTexture
    {
        private Dictionary<Direction, Texture2D> textures = new Dictionary<Direction, Texture2D>(TextureCount);

        internal const int TextureCount = 6;
        internal const int Size = 256;

        public SixFaceTexture(
            Texture2D right,
            Texture2D left,
            Texture2D top,
            Texture2D bottom,
            Texture2D forward,
            Texture2D back)
        {
            textures[Direction.Right] = right;
            textures[Direction.Left] = left;
            textures[Direction.Top] = top;
            textures[Direction.Bottom] = bottom;
            textures[Direction.Forward] = forward;
            textures[Direction.Back] = back;
        }

        internal Texture2D GetFace(Direction direction) => textures[direction];
    }
}