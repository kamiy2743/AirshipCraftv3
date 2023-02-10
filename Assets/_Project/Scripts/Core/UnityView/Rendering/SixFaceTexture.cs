using System.Collections.Generic;
using UnityEngine;

namespace UnityView.Rendering
{
    public record SixFaceTexture
    {
        readonly Dictionary<Face, Texture2D> _textures = new Dictionary<Face, Texture2D>(TextureCount);

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
            _textures[Face.Right] = right;
            _textures[Face.Left] = left;
            _textures[Face.Top] = top;
            _textures[Face.Bottom] = bottom;
            _textures[Face.Front] = front;
            _textures[Face.Back] = back;
        }

        internal Texture2D GetFace(Face face) => _textures[face];
    }
}