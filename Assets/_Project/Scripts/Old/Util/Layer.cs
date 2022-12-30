using UnityEngine;

namespace Util
{
    public class Layer
    {
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int Block = LayerMask.NameToLayer("Block");
        public static readonly int DropItem = LayerMask.NameToLayer("DropItem");
    }
}
