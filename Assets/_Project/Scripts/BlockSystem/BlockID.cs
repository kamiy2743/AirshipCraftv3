using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    public struct BlockID
    {
        public const int Max = 255;
        public const int EmptyID = -1;

        public readonly int value;
        public bool IsAir => value == 0;

        public BlockID(int value)
        {
            if (!IsValid(value)) throw new System.Exception($"{value} is invalid blockID");

            this.value = value;
        }

        private static bool IsValid(int value)
        {
            if (value < 0 || value > Max) return false;
            return true;
        }
    }
}
