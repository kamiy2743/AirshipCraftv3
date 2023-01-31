using System;
using Unity.Mathematics;
using Extensions;

namespace UnityView.ChunkRender
{
    internal enum Direction
    {
        Right = 1 << 0,
        Left = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        Forward = 1 << 4,
        Back = 1 << 5
    }

    internal static class DirectionExt
    {
        internal static readonly Direction[] Array = new Direction[]
        {
            Direction.Right,
            Direction.Left,
            Direction.Top,
            Direction.Bottom,
            Direction.Forward,
            Direction.Back
        };

        internal static int3 ToInt3(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return int3Ext.Right;
                case Direction.Left:
                    return int3Ext.Left;
                case Direction.Top:
                    return int3Ext.Top;
                case Direction.Bottom:
                    return int3Ext.Bottom;
                case Direction.Forward:
                    return int3Ext.Forward;
                case Direction.Back:
                    return int3Ext.Back;
            }

            throw new Exception("実装漏れ");
        }

        internal static Direction Flip(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Top:
                    return Direction.Bottom;
                case Direction.Bottom:
                    return Direction.Top;
                case Direction.Forward:
                    return Direction.Back;
                case Direction.Back:
                    return Direction.Forward;
            }

            throw new Exception("実装漏れ");
        }
    }
}