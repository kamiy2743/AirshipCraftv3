using ACv3.Domain.Items;

namespace ACv3.Domain.Inventories
{
    public record GrabbingInventoryItem
    {
        public readonly IItem Item;
        public readonly Amount Amount;

        public GrabbingInventoryItem(IItem item, Amount amount)
        {
            Item = item;
            Amount = amount;
        }

        public static GrabbingInventoryItem Empty => new(new EmptyItem(), Amount.Empty);
    }
}