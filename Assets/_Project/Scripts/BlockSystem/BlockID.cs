using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    public enum BlockID
    {
        Empty = -1,
        Air = 0,
        Dirt = 1,
        Grass = 2,
    }

    public static class BlockIDExt
    {
        public const int MaxValue = 255;
    }
}
