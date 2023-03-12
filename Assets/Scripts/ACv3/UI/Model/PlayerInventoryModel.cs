using System;
using ACv3.Domain.Inventories;
using UniRx;

namespace ACv3.UI.Model
{
    public class PlayerInventoryModel
    {
        readonly ReactiveProperty<bool> isSelected = new(false);
        readonly ReactiveProperty<PlayerInventorySlotId> selectedSlotId = new(PlayerInventorySlotId.Default());

        public IObservable<bool> IsSelectedAsObservable => isSelected;
        public IObservable<PlayerInventorySlotId> SelectedSlotIdAsObservable => selectedSlotId;

        public void SetIsSelected(bool isSelected) => this.isSelected.Value = isSelected; 
        public void SetSelectedSlotId(PlayerInventorySlotId slotId) => selectedSlotId.Value = slotId;
    }
}