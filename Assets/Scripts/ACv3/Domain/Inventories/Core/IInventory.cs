using System;

namespace ACv3.Domain.Inventories
{
    public interface IInventory
    {
        InventoryId InventoryId { get; }
        Slot GetSlot(GlobalInventorySlotId slotId);
        void SetSlot(GlobalInventorySlotId slotId, Slot slot);
        IObservable<(GlobalInventorySlotId slotId, Slot slot)> OnSlotUpdated { get; }
    }
}