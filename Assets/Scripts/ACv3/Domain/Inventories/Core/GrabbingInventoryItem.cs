using ACv3.Domain.Items;

namespace ACv3.Domain.Inventories
{
    public record GrabbingInventoryItem
    {
        public readonly StackItem Item;

        public GrabbingInventoryItem(StackItem item)
        {
            Item = item;
        }

        public static GrabbingInventoryItem Empty => new(StackItem.Empty);
    }
}