using UnityEngine.Assertions;

namespace ACv3.Domain.Items
{
    public record StackItem
    {
        readonly IItem item;
        public readonly Amount Amount;
        public ItemId ItemId => item.ItemId;

        public static StackItem Empty => new(new EmptyItem(), Amount.Empty);

        public StackItem(IItem item, Amount amount)
        {
            Assert.IsTrue(amount <= item.MaxAmount);
            this.item = item;
            Amount = amount;
        }

        public (StackItem primaryItem, StackItem subItem) Merge(StackItem other)
        {
            if (ItemId != other.ItemId)
            {
                return (this, other);
            }
            
            var sumAmount = Amount + other.Amount;
            
            if (sumAmount <= item.MaxAmount)
            {
                return (new StackItem(item, sumAmount), Empty);
            }

            var primaryItem = new StackItem(item, item.MaxAmount);
            var subItem = new StackItem(item, sumAmount - item.MaxAmount);
            return (primaryItem, subItem);
        }
    }
}