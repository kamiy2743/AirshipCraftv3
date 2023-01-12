using Unity.Mathematics;
using Extensions;

namespace Domain
{
    internal enum AdjacentSurface
    {
        Right = 0,
        Left = 1,
        Top = 2,
        Bottom = 3,
        Forward = 4,
        Back = 5
    }

    internal static class AdjacentSurfaceExt
    {
        internal static int3 ToInt3(this AdjacentSurface surface)
        {
            switch (surface)
            {
                case AdjacentSurface.Right:
                    return int3Ext.Right;
                case AdjacentSurface.Left:
                    return int3Ext.Left;
                case AdjacentSurface.Top:
                    return int3Ext.Top;
                case AdjacentSurface.Bottom:
                    return int3Ext.Bottom;
                case AdjacentSurface.Forward:
                    return int3Ext.Forward;
                case AdjacentSurface.Back:
                    return int3Ext.Back;
            }

            return int3.zero;
        }

        internal static AdjacentSurface Flip(this AdjacentSurface surface)
        {
            switch (surface)
            {
                case AdjacentSurface.Right:
                    return AdjacentSurface.Left;
                case AdjacentSurface.Left:
                    return AdjacentSurface.Right;
                case AdjacentSurface.Top:
                    return AdjacentSurface.Bottom;
                case AdjacentSurface.Bottom:
                    return AdjacentSurface.Top;
                case AdjacentSurface.Forward:
                    return AdjacentSurface.Back;
                case AdjacentSurface.Back:
                    return AdjacentSurface.Forward;
            }

            return AdjacentSurface.Right;
        }
    }
}