using System;
using ACv3.Domain.Inventories;
using ACv3.Extensions;
using UniRx;

namespace ACv3.Domain.Windows
{
    public class PlayerWindow : IWindow, IInventory
    {
        WindowId IWindow.WindowId => WindowId.PlayerWindow;
        InventoryId IInventory.InventoryId => InventoryId.PlayerInventory;

        readonly ReactiveDictionary<PlayerInventorySlotId, Slot> slots = new();
        readonly IObservable<(GlobalInventorySlotId slotId, Slot slot)> onSlotUpdatedObservable;

        public PlayerWindow()
        {
            onSlotUpdatedObservable = Observable.Merge(
                    slots.ObserveReplace().Select(e => ((GlobalInventorySlotId)e.Key, e.NewValue)),
                    slots.ObserveAdd().Select(e => ((GlobalInventorySlotId)e.Key, e.Value)))
                .AsReplayObservable();

            for (int line = 0; line < PlayerInventorySlotId.LineCount; line++)
            {
                for (int row = 0; row < PlayerInventorySlotId.RowCount; row++)
                {
                    slots[new PlayerInventorySlotId(line, row)] = Slot.Empty;
                }
            }
        }

        Slot IInventory.GetSlot(GlobalInventorySlotId slotId) => slots[slotId];
        void IInventory.SetSlot(GlobalInventorySlotId slotId, Slot slot) => slots[slotId] = slot;
        IObservable<(GlobalInventorySlotId slotId, Slot slot)> IInventory.OnSlotUpdated => onSlotUpdatedObservable;
    }
}