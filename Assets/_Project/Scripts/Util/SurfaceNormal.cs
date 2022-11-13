using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Util
{
    [System.Flags]
    public enum SurfaceNormal
    {
        Empty = byte.MaxValue,
        Zero = 0,
        Right = 1 << 0,
        Left = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        Forward = 1 << 4,
        Back = 1 << 5,
    }

    public static class SurfaceNormalExt
    {
        public static readonly SurfaceNormal[] Array = new SurfaceNormal[] {
            SurfaceNormal.Right,
            SurfaceNormal.Left,
            SurfaceNormal.Top,
            SurfaceNormal.Bottom,
            SurfaceNormal.Forward,
            SurfaceNormal.Back,
        };

        public static SurfaceNormal FromIndex(int index)
        {
            return (SurfaceNormal)(1 << index);
        }

        private static readonly int3 i3right = new int3(1, 0, 0);
        private static readonly int3 i3left = new int3(-1, 0, 0);
        private static readonly int3 i3top = new int3(0, 1, 0);
        private static readonly int3 i3bottom = new int3(0, -1, 0);
        private static readonly int3 i3forward = new int3(0, 0, 1);
        private static readonly int3 i3back = new int3(0, 0, -1);

        public static int3 ToInt3(this SurfaceNormal surface)
        {
            switch (surface)
            {
                case SurfaceNormal.Right:
                    return i3right;
                case SurfaceNormal.Left:
                    return i3left;
                case SurfaceNormal.Top:
                    return i3top;
                case SurfaceNormal.Bottom:
                    return i3bottom;
                case SurfaceNormal.Forward:
                    return i3forward;
                case SurfaceNormal.Back:
                    return i3back;
            }

            return int3.zero;
        }

        /// <summary>
        /// 呼ぶだけでは値が反映されないので戻り値を再代入してください
        /// </summary>
        public static SurfaceNormal Add(this SurfaceNormal surface, SurfaceNormal value)
        {
            return surface | value;
        }

        public static bool Contains(this SurfaceNormal surface, SurfaceNormal value)
        {
            return (surface & value) > 0;
        }

        private const byte FullValue = 0b111111;
        public static bool IsFull(this SurfaceNormal surface)
        {
            return (byte)surface == FullValue;
        }
    }
}