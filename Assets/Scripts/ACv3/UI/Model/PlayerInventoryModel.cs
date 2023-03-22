using System;
using ACv3.Domain.Inventories;
using ACv3.Domain.Items;
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
        
        readonly ReactiveProperty<PlayerInventorySlotId> selectedSlotId = new(PlayerInventorySlotId.Empty);
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
                    slots.Add(new PlayerInventorySlotId(line, row), Slot.Empty);
                }
            }
            
            // デバッグ
            slots[new PlayerInventorySlotId(0, 0)] = new Slot(new Dirt(), new Amount(24));
            slots[new PlayerInventorySlotId(1, 4)] = new Slot(new Dirt(), new Amount(45));
            slots[new PlayerInventorySlotId(3, 6)] = new Slot(new Dirt(), new Amount(80));
        }

        void IInventory.Open() => isOpened.Value = true;
        void IInventory.Close() => isOpened.Value = false;
        
        public void SetSelectedSlotId(PlayerInventorySlotId slotId) => selectedSlotId.Value = slotId;

        public void InvokeSlotClickedEvent(PlayerInventorySlotId slotId) => slotClickedSubject.OnNext(slotId.ToGlobalInventorySlotId());
        IObservable<GlobalInventorySlotId> IInventory.OnSlotClicked() => slotClickedSubject;
        
        Slot IInventory.GetSlot(GlobalInventorySlotId slotId) => slots[PlayerInventorySlotId.FromGlobalInventorySlotId(slotId)];
        void IInventory.SetSlot(GlobalInventorySlotId slotId, Slot slot) => slots[PlayerInventorySlotId.FromGlobalInventorySlotId(slotId)] = slot;
    }
}