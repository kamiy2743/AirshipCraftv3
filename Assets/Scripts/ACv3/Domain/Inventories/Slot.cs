using ACv3.Domain.Items;
using UnityEngine;
using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record Slot
    {
        public readonly IItem Item;
        public readonly Amount Amount;
        public readonly Texture2D Texture;

        public Slot(IItem item, Amount amount, Texture2D texture)
        {
            Assert.IsTrue(amount <= item.MaxAmount);
            Item = item;
            Amount = amount;
            Texture = texture;
        }

        public static Slot Empty() => new Slot(new EmptyItem(), Amount.Empty(), null);
    }
}