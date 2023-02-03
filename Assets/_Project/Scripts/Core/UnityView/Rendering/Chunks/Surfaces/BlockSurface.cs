namespace UnityView.Rendering.Chunks
{
    internal record BlockSurface
    {
        private readonly byte surfacesByte;
        internal byte SurfaceByteDebug => surfacesByte;

        internal readonly bool hasRenderingSurface;

        internal static readonly BlockSurface Empty = CreateEmpty();

        internal BlockSurface()
        {
            surfacesByte = 0;
            hasRenderingSurface = false;
        }

        internal BlockSurface(params Direction[] directions)
        {
            var result = new BlockSurface();
            foreach (var surface in directions)
            {
                result += surface;
            }

            surfacesByte = result.surfacesByte;
            hasRenderingSurface = true;
        }

        private BlockSurface(byte surfacesByte)
        {
            this.surfacesByte = surfacesByte;
            hasRenderingSurface = surfacesByte > 0;
        }

        private static BlockSurface CreateEmpty()
        {
            return new BlockSurface((byte)0);
        }

        internal bool Contains(Direction direction)
        {
            return (surfacesByte & (byte)direction) > 0;
        }

        public static BlockSurface operator +(BlockSurface renderingSurface, Direction direction)
        {
            var result = renderingSurface.surfacesByte | (byte)direction;
            return new BlockSurface((byte)result);
        }

        public static BlockSurface operator -(BlockSurface renderingSurface, Direction direction)
        {
            var result = renderingSurface.surfacesByte ^ (byte)direction;
            return new BlockSurface((byte)result);
        }
    }
}