namespace UnityView.ChunkRendering.RenderingSurface
{
    internal record BlockRenderingSurface
    {
        private readonly byte surfacesByte;
        internal byte SurfaceByteDebug => surfacesByte;

        internal readonly bool hasRenderingSurface;

        internal BlockRenderingSurface()
        {
            surfacesByte = 0;
            hasRenderingSurface = false;
        }

        internal BlockRenderingSurface(params Direction[] directions)
        {
            var result = new BlockRenderingSurface();
            foreach (var surface in directions)
            {
                result += surface;
            }

            surfacesByte = result.surfacesByte;
            hasRenderingSurface = true;
        }

        private BlockRenderingSurface(byte surfacesByte)
        {
            this.surfacesByte = surfacesByte;
            hasRenderingSurface = surfacesByte > 0;
        }

        internal bool Contains(Direction direction)
        {
            return (surfacesByte & (byte)direction) > 0;
        }

        public static BlockRenderingSurface operator +(BlockRenderingSurface renderingSurface, Direction direction)
        {
            var result = renderingSurface.surfacesByte | (byte)direction;
            return new BlockRenderingSurface((byte)result);
        }

        public static BlockRenderingSurface operator -(BlockRenderingSurface renderingSurface, Direction direction)
        {
            var result = renderingSurface.surfacesByte ^ (byte)direction;
            return new BlockRenderingSurface((byte)result);
        }
    }
}