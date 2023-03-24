using ACv3.Domain.Items;

namespace ACv3.Domain.Inventories
{
    public record Slot
    {
        public readonly StackItem Item;
        
        public Slot(StackItem item)
        {
            Item = item;
        }

        public static Slot Empty => new(StackItem.Empty);
        public bool IsEmpty => this == Empty;
    }
}