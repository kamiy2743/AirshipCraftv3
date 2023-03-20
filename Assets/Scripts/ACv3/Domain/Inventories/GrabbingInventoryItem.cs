using ACv3.Domain.Items;
using UnityEngine;

namespace ACv3.Domain.Inventories
{
    public record GrabbingInventoryItem
    {
        public readonly IItem Item;
        public readonly Amount Amount;
        public readonly Texture2D Texture;

        GrabbingInventoryItem(IItem item, Amount amount, Texture2D texture)
        {
            Item = item;
            Amount = amount;
            Texture = texture;
        }

        public GrabbingInventoryItem(Slot slot)
        {
            Item = slot.Item;
            Amount = slot.Amount;
            Texture = slot.Texture;
        }

        public static GrabbingInventoryItem Empty() => new(new EmptyItem(), Amount.Empty(), null);
    }
}