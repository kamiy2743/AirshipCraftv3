using System;
using ACv3.Domain.Inventories;
using UniRx;

namespace ACv3.UI.Model
{
    public class PlayerInventoryModel
    {
        readonly ReactiveDictionary<PlayerInventorySlotId, Slot> slots;
        public IObservable<(PlayerInventorySlotId slotId, Slot slot)> OnUpdateSlot =>
            slots.ObserveReplace().Select(e => (e.Key, e.NewValue));

        readonly ReactiveProperty<bool> isOpened = new(false);
        public IReadOnlyReactiveProperty<bool> IsOpened => isOpened;
        
        readonly ReactiveProperty<bool> isSelected = new(false);
        public IObservable<bool> IsSelectedAsObservable => isSelected;
        
        readonly ReactiveProperty<PlayerInventorySlotId> selectedSlotId = new(PlayerInventorySlotId.Default());
        public IObservable<PlayerInventorySlotId> SelectedSlotIdAsObservable => selectedSlotId;

        PlayerInventoryModel()
        {
            slots = new ReactiveDictionary<PlayerInventorySlotId, Slot>();
            for (int line = 0; line < PlayerInventorySlotId.LineCount; line++)
            {
                for (int row = 0; row < PlayerInventorySlotId.RowCount; row++)
                {
                    slots.Add(new PlayerInventorySlotId(line, row), Slot.Empty());
                }
            }
        }
        
        public void SetIsSelected(bool isSelected) => this.isSelected.Value = isSelected; 
        public void SetSelectedSlotId(PlayerInventorySlotId slotId) => selectedSlotId.Value = slotId;

        public void Open() => isOpened.Value = true;
        public void Close() => isOpened.Value = false;

        public void SetSlot(PlayerInventorySlotId slotId, Slot slot)
        {
            slots[slotId] = slot;
        }
    }
}