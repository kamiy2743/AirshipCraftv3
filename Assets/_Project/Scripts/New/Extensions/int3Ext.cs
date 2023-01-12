using Unity.Mathematics;

namespace Extensions
{
    public static class int3Ext
    {
        public static readonly int3 Right = new int3(1, 0, 0);
        public static readonly int3 Left = new int3(-1, 0, 0);
        public static readonly int3 Top = new int3(0, 1, 0);
        public static readonly int3 Bottom = new int3(0, -1, 0);
        public static readonly int3 Forward = new int3(0, 0, 1);
        public static readonly int3 Back = new int3(0, 0, -1);
    }
}