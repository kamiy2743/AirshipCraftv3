using UnityEngine;
using System.Collections.Generic;

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
        private static SurfaceNormal[] _array;
        public static SurfaceNormal[] Array
        {
            get
            {
                if (_array is not null) return _array;

                var list = new List<SurfaceNormal>(6);
                foreach (SurfaceNormal surface in System.Enum.GetValues(typeof(SurfaceNormal)))
                {
                    if (surface == SurfaceNormal.Empty) continue;
                    if (surface == SurfaceNormal.Zero) continue;
                    list.Add(surface);
                }

                _array = list.ToArray();
                return _array;
            }
        }

        public static SurfaceNormal FromIndex(int index)
        {
            return (SurfaceNormal)(1 << index);
        }

        public static Vector3 ToVector3(this SurfaceNormal surface)
        {
            switch (surface)
            {
                case SurfaceNormal.Right:
                    return Vector3.right;
                case SurfaceNormal.Left:
                    return Vector3.left;
                case SurfaceNormal.Top:
                    return Vector3.up;
                case SurfaceNormal.Bottom:
                    return Vector3.down;
                case SurfaceNormal.Forward:
                    return Vector3.forward;
                case SurfaceNormal.Back:
                    return Vector3.back;
            }

            return Vector3.zero;
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