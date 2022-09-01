using UnityEngine;
using System.Collections.Generic;

namespace Util
{
    public enum SurfaceNormal
    {
        Right,
        Left,
        Top,
        Bottom,
        Forward,
        Back
    }

    public static class SurfaceNormalExt
    {
        public static readonly int EnumCount = System.Enum.GetValues(typeof(SurfaceNormal)).Length;

        public static readonly IReadOnlyList<SurfaceNormal> List = _list ??= ToList();
        private static List<SurfaceNormal> _list;
        private static List<SurfaceNormal> ToList()
        {
            var list = new List<SurfaceNormal>(EnumCount);
            foreach (SurfaceNormal surface in System.Enum.GetValues(typeof(SurfaceNormal)))
            {
                list.Add(surface);
            }
            return list;
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
    }
}