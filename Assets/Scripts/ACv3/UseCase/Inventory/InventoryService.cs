using System;
using ACv3.Domain.Inventories;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UseCase.Inventory
{
    public class InventoryService
    {
        readonly WindowProvider windowProvider;
        readonly ReactiveProperty<GrabbingInventoryItem> grabbingItem = new(GrabbingInventoryItem.Empty);

        [Inject]
        InventoryService(WindowProvider windowProvider)
        {
            this.windowProvider = windowProvider;
        }

        public IObservable<GrabbingInventoryItem> OnGrabItemStarted() =>
            grabbingItem
                .Where(item => !item.IsEmpty)
                .Publish()
                .RefCount();
        
        public IObservable<Unit> OnGrabItemEnded() =>
            grabbingItem
                .Where(item => item.IsEmpty)
                .AsUnitObservable()
                .Publish()
                .RefCount();
        
        readonly ReactiveProperty<GlobalInventorySlotId> selectedSlotId = new(GlobalInventorySlotId.Empty);
        public IReadOnlyReactiveProperty<GlobalInventorySlotId> SelectedSlotId => selectedSlotId;
        
        public void SetSelectedSlotId(GlobalInventorySlotId slotId) => selectedSlotId.Value = slotId;

        public void GrabStartOrEndRequest(GlobalInventorySlotId slotId)
        {
            if (grabbingItem.Value.IsEmpty)
            {
                GrabStart(slotId);
            }
            else
            {
                GrabEnd(slotId);
            }
        }
        
        void GrabStart(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab start: " + slotId);
            
            var inventory = windowProvider.GetInventories()[slotId.InventoryId];
            
            var slot = inventory.GetSlot(slotId);
            if (slot.IsEmpty)
            {
                return;
            }
            
            grabbingItem.Value = new GrabbingInventoryItem(slot.Item);
            inventory.SetSlot(slotId, Slot.Empty);
        }

        void GrabEnd(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab end: " + slotId);

            var inventory = windowProvider.GetInventories()[slotId.InventoryId];
            
            var placeSlot = inventory.GetSlot(slotId);
            if (placeSlot.IsEmpty)
            {
                inventory.SetSlot(slotId, new Slot(grabbingItem.Value.Item));
                grabbingItem.Value = GrabbingInventoryItem.Empty;
                return;
            }
            
            var (primaryItem, subItem) = grabbingItem.Value.Item.Merge(placeSlot.Item);
            inventory.SetSlot(slotId, new Slot(primaryItem));

            if (subItem.IsEmpty)
            {
                grabbingItem.Value = GrabbingInventoryItem.Empty;
            }
            else
            {
                grabbingItem.Value = new GrabbingInventoryItem(subItem);
            }
        }
    }
}