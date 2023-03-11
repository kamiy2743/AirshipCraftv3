using Unity.Mathematics;

namespace ACv3.Extensions
{
    public static class int3Ext
    {
        public static readonly int3 PositiveX = new int3(1, 0, 0);
        public static readonly int3 NegativeX = new int3(-1, 0, 0);
        public static readonly int3 PositiveY = new int3(0, 1, 0);
        public static readonly int3 NegativeY = new int3(0, -1, 0);
        public static readonly int3 PositiveZ = new int3(0, 0, 1);
        public static readonly int3 NegativeZ = new int3(0, 0, -1);
    }
}