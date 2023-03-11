using System;
using System.Linq;

namespace Domain
{
    public enum BlockType
    {
        Air,
        Grass,
        Dirt,
        Stone,
        MachineUnitCore
    }

    public static class BlockTypeExt
    {
        static BlockType[] _array;
        public static BlockType[] Array => _array ??= Enum.GetValues(typeof(BlockType)).Cast<BlockType>().ToArray();
    }
}