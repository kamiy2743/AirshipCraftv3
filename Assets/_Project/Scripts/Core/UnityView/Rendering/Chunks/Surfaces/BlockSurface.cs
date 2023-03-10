using Domain;

namespace UnityView.Rendering.Chunks
{
    record BlockSurface
    {
        readonly byte _surfacesByte;
        internal byte SurfaceByteDebug => _surfacesByte;

        internal readonly bool HasRenderingSurface;

        internal static readonly BlockSurface Empty = CreateEmpty();

        internal BlockSurface()
        {
            _surfacesByte = 0;
            HasRenderingSurface = false;
        }

        internal BlockSurface(params Face[] faces)
        {
            var result = new BlockSurface();
            foreach (var face in faces)
            {
                result += face;
            }

            _surfacesByte = result._surfacesByte;
            HasRenderingSurface = true;
        }

        BlockSurface(byte surfacesByte)
        {
            _surfacesByte = surfacesByte;
            HasRenderingSurface = surfacesByte > 0;
        }

        static BlockSurface CreateEmpty()
        {
            return new BlockSurface((byte)0);
        }

        internal bool Contains(Face face)
        {
            return (_surfacesByte & (byte)face) > 0;
        }

        public static BlockSurface operator +(BlockSurface renderingSurface, Face face)
        {
            var result = renderingSurface._surfacesByte | (byte)face;
            return new BlockSurface((byte)result);
        }

        public static BlockSurface operator -(BlockSurface renderingSurface, Face face)
        {
            var result = renderingSurface._surfacesByte ^ (byte)face;
            return new BlockSurface((byte)result);
        }
    }
}