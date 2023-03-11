using System.Collections.Generic;
using UnityEngine;

namespace UnityView.Rendering
{
    public record SixFaceTexture
    {
        readonly Dictionary<Face, Texture2D> textures = new(TextureCount);

        internal const int TextureCount = 6;
        internal const int Size = 256;

        public SixFaceTexture(
            Texture2D right,
            Texture2D left,
            Texture2D top,
            Texture2D bottom,
            Texture2D front,
            Texture2D back)
        {
            textures[Face.Right] = right;
            textures[Face.Left] = left;
            textures[Face.Top] = top;
            textures[Face.Bottom] = bottom;
            textures[Face.Front] = front;
            textures[Face.Back] = back;
        }

        internal Texture2D GetFace(Face face) => textures[face];
    }
}