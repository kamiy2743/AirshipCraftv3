using System;
using Unity.Mathematics;
using Extensions;

namespace Domain
{
    public enum Direction
    {
        Right = 1 << 0,
        Left = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
        Forward = 1 << 4,
        BackWard = 1 << 5
    }

    public static class DirectionExt
    {
        public const int ElemCount = 6;

        public static readonly Direction[] Array = new Direction[]
        {
            Direction.Right,
            Direction.Left,
            Direction.Up,
            Direction.Down,
            Direction.Forward,
            Direction.BackWard
        };

        public static int3 ToInt3(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return int3Ext.PositiveX;
                case Direction.Left:
                    return int3Ext.NegativeX;
                case Direction.Up:
                    return int3Ext.PositiveY;
                case Direction.Down:
                    return int3Ext.NegativeY;
                case Direction.Forward:
                    return int3Ext.PositiveZ;
                case Direction.BackWard:
                    return int3Ext.NegativeZ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static Direction Flip(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Forward:
                    return Direction.BackWard;
                case Direction.BackWard:
                    return Direction.Forward;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}