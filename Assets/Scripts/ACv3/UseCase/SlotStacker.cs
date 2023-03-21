using ACv3.Domain.Inventories;

namespace ACv3.UseCase
{
    public static class SlotStacker
    {
        public static (Slot primarySlot, Slot subSlot) Stack(Slot primary, Slot sub)
        {
            if (primary.Item.ItemId != sub.Item.ItemId)
            {
                return (primary, sub);
            }

            var item = primary.Item;
            var sumAmount = primary.Amount + sub.Amount;

            if (sumAmount <= item.MaxAmount)
            {
                return (new Slot(item, sumAmount), Slot.Empty());
            }

            return (new Slot(item, primary.Item.MaxAmount), new Slot(item, sumAmount - item.MaxAmount));
        }
    }
}