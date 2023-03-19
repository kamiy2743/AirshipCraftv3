namespace ACv3.Presentation.Rendering.Chunks
{
    record BlockSurface
    {
        readonly byte surfacesByte;
        internal byte SurfaceByteDebug => surfacesByte;

        internal readonly bool hasRenderingSurface;

        internal static readonly BlockSurface Empty = CreateEmpty();

        internal BlockSurface()
        {
            surfacesByte = 0;
            hasRenderingSurface = false;
        }

        internal BlockSurface(params Face[] faces)
        {
            var result = new BlockSurface();
            foreach (var face in faces)
            {
                result += face;
            }

            surfacesByte = result.surfacesByte;
            hasRenderingSurface = true;
        }

        BlockSurface(byte surfacesByte)
        {
            this.surfacesByte = surfacesByte;
            hasRenderingSurface = surfacesByte > 0;
        }

        static BlockSurface CreateEmpty()
        {
            return new BlockSurface((byte)0);
        }

        internal bool Contains(Face face)
        {
            return (surfacesByte & (byte)face) > 0;
        }

        public static BlockSurface operator +(BlockSurface renderingSurface, Face face)
        {
            var result = renderingSurface.surfacesByte | (byte)face;
            return new BlockSurface((byte)result);
        }

        public static BlockSurface operator -(BlockSurface renderingSurface, Face face)
        {
            var result = renderingSurface.surfacesByte ^ (byte)face;
            return new BlockSurface((byte)result);
        }
    }
}