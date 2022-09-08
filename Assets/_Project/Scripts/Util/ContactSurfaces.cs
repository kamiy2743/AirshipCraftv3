namespace Util
{
    public struct ContactSurfaces
    {
        private static readonly uint[] Masks = new uint[] {
            0b100000,
            0b010000,
            0b001000,
            0b000100,
            0b000010,
            0b000001,
        };
        private const uint FullValue = 0b111111;

        public uint value;

        public bool IsFull => value == FullValue;

        public void Add(SurfaceNormal surface)
        {
            if (Contains(surface)) return;
            value += Masks[(int)surface];
        }

        public void Remove(SurfaceNormal surface)
        {
            if (!Contains(surface)) return;
            value -= Masks[(int)surface];
        }

        public bool Contains(SurfaceNormal surface)
        {
            return (value & Masks[(int)surface]) > 0;
        }
    }
}
