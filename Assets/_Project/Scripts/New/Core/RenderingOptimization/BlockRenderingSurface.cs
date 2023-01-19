namespace RenderingOptimization
{
    internal record BlockRenderingSurface
    {
        private readonly byte surfacesByte;
        internal byte SurfaceByteDebug => surfacesByte;

        internal BlockRenderingSurface()
        {
            surfacesByte = 0;
        }

        internal BlockRenderingSurface(params Surface[] surfaces)
        {
            var result = new BlockRenderingSurface();
            foreach (var surface in surfaces)
            {
                result += surface;
            }

            surfacesByte = result.surfacesByte;
        }

        private BlockRenderingSurface(byte surfacesByte)
        {
            this.surfacesByte = surfacesByte;
        }

        internal bool Contains(Surface surface)
        {
            return (surfacesByte & (byte)surface) == 1;
        }

        public static BlockRenderingSurface operator +(BlockRenderingSurface renderingSurface, Surface surface)
        {
            var result = renderingSurface.surfacesByte | (byte)surface;
            return new BlockRenderingSurface((byte)result);
        }

        public static BlockRenderingSurface operator -(BlockRenderingSurface renderingSurface, Surface surface)
        {
            var result = renderingSurface.surfacesByte ^ (byte)surface;
            return new BlockRenderingSurface((byte)result);
        }
    }
}