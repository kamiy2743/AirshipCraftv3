using System;
using ACv3.Domain.Inventories;
using ACv3.UseCase;
using UniRx;
using Zenject;

namespace ACv3.UI.Model
{
    public class PlayerInventoryModel : IInventory
    {
        InventoryId IInventory.Id => InventoryId.PlayerInventory;
        readonly InventoryBroker inventoryBroker;

        readonly ReactiveDictionary<PlayerInventorySlotId, Slot> slots = new();
        public IObservable<(PlayerInventorySlotId slotId, Slot slot)> OnUpdateSlot
        {
            get
            {
                return Observable.Merge(
                    slots.ObserveReplace().Select(e => (e.Key, e.NewValue)),
                    slots.ObserveAdd().Select(e => (e.Key, e.Value)));
            }
        }

        readonly ReactiveProperty<bool> isOpened = new(false);
        public IReadOnlyReactiveProperty<bool> IsOpened => isOpened.DistinctUntilChanged().ToReadOnlyReactiveProperty();
        
        readonly ReactiveProperty<bool> isSelected = new(false);
        public IReadOnlyReactiveProperty<bool> IsSelected => isOpened;
        
        readonly ReactiveProperty<PlayerInventorySlotId> selectedSlotId = new(PlayerInventorySlotId.Default());
        public IReadOnlyReactiveProperty<PlayerInventorySlotId> SelectedSlotId => selectedSlotId;

        readonly Subject<GlobalInventorySlotId> slotClickedSubject = new();

        [Inject]
        PlayerInventoryModel(InventoryBroker inventoryBroker)
        {
            this.inventoryBroker = inventoryBroker;
        }

        public void Initialize()
        {
            inventoryBroker.AddInventory(this);
            
            for (int line = 0; line < PlayerInventorySlotId.LineCount; line++)
            {
                for (int row = 0; row < PlayerInventorySlotId.RowCount; row++)
                {
                    slots.Add(new PlayerInventorySlotId(line, row), Slot.Empty());
                }
            }
        }

        void IInventory.Open() => isOpened.Value = true;
        void IInventory.Close() => isOpened.Value = false;

        public void InvokeSlotClickedEvent(PlayerInventorySlotId slotId) => slotClickedSubject.OnNext(new GlobalInventorySlotId(InventoryId.PlayerInventory, slotId));
        IObservable<GlobalInventorySlotId> IInventory.OnSlotClicked() => slotClickedSubject;
        
        public void SetIsSelected(bool isSelected) => this.isSelected.Value = isSelected; 
        public void SetSelectedSlotId(PlayerInventorySlotId slotId) => selectedSlotId.Value = slotId;
        public void SetSlot(PlayerInventorySlotId slotId, Slot slot) => slots[slotId] = slot;
    }
}