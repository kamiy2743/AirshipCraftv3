using ACv3.Domain.Items;
using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record Slot
    {
        public readonly IItem Item;
        public readonly Amount Amount;

        public Slot(IItem item, Amount amount)
        {
            Assert.IsTrue(amount <= item.MaxAmount);
            Item = item;
            Amount = amount;
        }

        public static Slot Empty() => new(new EmptyItem(), Amount.Empty());
    }
}