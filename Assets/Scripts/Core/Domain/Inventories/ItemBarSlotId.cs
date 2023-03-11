using UnityEngine;
using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record ItemBarSlotId
    {
        readonly int value;
        
        public const int Min = 0;
        public const int Max = 8;

        public ItemBarSlotId(int value)
        {
            Assert.IsTrue(value >= Min);
            Assert.IsTrue(value <= Max);
            
            this.value = value;
        }

        public int ToInt() => value;
        public static ItemBarSlotId CreateMin() => new(Min);
        
        public override string ToString() => $"ItemBarSlotId: {value}";
    }
}