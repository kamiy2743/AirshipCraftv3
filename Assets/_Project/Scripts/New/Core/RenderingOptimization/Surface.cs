using System;
using Unity.Mathematics;
using Extensions;

namespace RenderingOptimization
{
    internal enum Surface
    {
        Right = 1 << 0,
        Left = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        Forward = 1 << 4,
        Back = 1 << 5
    }

    internal static class SurfaceExt
    {
        internal static readonly Surface[] Array = new Surface[]
        {
            Surface.Right,
            Surface.Left,
            Surface.Top,
            Surface.Bottom,
            Surface.Forward,
            Surface.Back
        };

        internal static int3 ToInt3(this Surface surface)
        {
            switch (surface)
            {
                case Surface.Right:
                    return int3Ext.Right;
                case Surface.Left:
                    return int3Ext.Left;
                case Surface.Top:
                    return int3Ext.Top;
                case Surface.Bottom:
                    return int3Ext.Bottom;
                case Surface.Forward:
                    return int3Ext.Forward;
                case Surface.Back:
                    return int3Ext.Back;
            }

            throw new Exception("実装漏れ");
        }

        internal static Surface Flip(this Surface surface)
        {
            switch (surface)
            {
                case Surface.Right:
                    return Surface.Left;
                case Surface.Left:
                    return Surface.Right;
                case Surface.Top:
                    return Surface.Bottom;
                case Surface.Bottom:
                    return Surface.Top;
                case Surface.Forward:
                    return Surface.Back;
                case Surface.Back:
                    return Surface.Forward;
            }

            throw new Exception("実装漏れ");
        }
    }
}