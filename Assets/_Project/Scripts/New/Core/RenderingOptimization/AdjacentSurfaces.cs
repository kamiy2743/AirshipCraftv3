namespace RenderingOptimization
{
    internal class AdjacentSurfaces
    {
        private readonly byte surfaces;

        internal static readonly AdjacentSurface[] Array = new AdjacentSurface[]
        {
            AdjacentSurface.Right,
            AdjacentSurface.Left,
            AdjacentSurface.Top,
            AdjacentSurface.Bottom,
            AdjacentSurface.Forward,
            AdjacentSurface.Back
        };

        internal AdjacentSurfaces()
        {
            surfaces = 0;
        }

        private AdjacentSurfaces(byte surfaces)
        {
            this.surfaces = surfaces;
        }

        internal AdjacentSurfaces Add(AdjacentSurface surface)
        {
            var result = surfaces | (byte)surface;
            return new AdjacentSurfaces((byte)result);
        }

        internal AdjacentSurfaces Remove(AdjacentSurface surface)
        {
            var result = surfaces ^ (byte)surface;
            return new AdjacentSurfaces((byte)result);
        }
    }
}