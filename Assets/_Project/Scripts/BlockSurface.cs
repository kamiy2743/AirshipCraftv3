using UnityEngine;

namespace BlockSystem
{
    public enum BlockSurface
    {
        Right,
        Left,
        Top,
        Bottom,
        Forward,
        Back
    }

    public static class BlockSurfaceExt
    {
        public static Vector3 ToVector3(this BlockSurface surface)
        {
            switch (surface)
            {
                case BlockSurface.Right:
                    return Vector3.right;
                case BlockSurface.Left:
                    return Vector3.left;
                case BlockSurface.Top:
                    return Vector3.up;
                case BlockSurface.Bottom:
                    return Vector3.down;
                case BlockSurface.Forward:
                    return Vector3.forward;
                case BlockSurface.Back:
                    return Vector3.back;
            }

            return Vector3.zero;
        }
    }
}