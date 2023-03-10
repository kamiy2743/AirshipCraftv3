using System;
using Domain;

namespace UnityView.Rendering
{
    public enum Face
    {
        Right = 1 << 0,
        Left = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        Front = 1 << 4,
        Back = 1 << 5
    }

    public static class FaceExt
    {
        public const int ElemCount = 6;

        public static readonly Face[] Array = new Face[]
        {
            Face.Right,
            Face.Left,
            Face.Top,
            Face.Bottom,
            Face.Front,
            Face.Back
        };

        public static Face Parse(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return Face.Right;
                case Direction.Left:
                    return Face.Left;
                case Direction.Up:
                    return Face.Top;
                case Direction.Down:
                    return Face.Bottom;
                case Direction.Forward:
                    return Face.Front;
                case Direction.BackWard:
                    return Face.Back;
            }

            throw new Exception("実装漏れ");
        }
    }
}