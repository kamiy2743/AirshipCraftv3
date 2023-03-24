using System;
using System.Collections.Generic;
using ACv3.Domain.Inventories;
using ACv3.Domain.Items;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UseCase
{
    // UseCaseがHotなクラスなのは何かおかしい
    public class GrabInventoryItemService : IInitializable, IDisposable
    {
        readonly InventoryBroker inventoryBroker;
        readonly Dictionary<InventoryId, IDisposable> disposableDictionary = new();
        readonly CompositeDisposable compositeDisposable = new();

        readonly ReactiveProperty<GrabbingInventoryItem> grabbingItem = new(GrabbingInventoryItem.Empty);
        public IReadOnlyReactiveProperty<GrabbingInventoryItem> GrabbingItem => grabbingItem.DistinctUntilChanged().ToReadOnlyReactiveProperty();

        [Inject]
        GrabInventoryItemService(InventoryBroker inventoryBroker)
        {
            this.inventoryBroker = inventoryBroker;
        }

        void IInitializable.Initialize()
        {
            foreach (var item in inventoryBroker.Inventories)
            {
                Setup(item.Key, item.Value);
            }

            inventoryBroker.ObserveAdd()
                .Subscribe(value => Setup(value.id, value.inventory))
                .AddTo(compositeDisposable);

            inventoryBroker.ObserveRemove()
                .Subscribe(id =>
                {
                    var disposable = disposableDictionary[id];
                    compositeDisposable.Remove(disposable);
                    disposableDictionary.Remove(id);
                    disposable.Dispose();
                })
                .AddTo(compositeDisposable);
        }

        void Setup(InventoryId id, IInventory inventory)
        { 
            var disposable = inventory.OnSlotClicked()
                .Subscribe(slotId =>
                {
                    if (grabbingItem.Value.IsEmpty)
                    {
                        GrabStart(slotId);
                    }
                    else
                    {
                        GrabEnd(slotId);
                    }
                }); 
            disposableDictionary.Add(id, disposable);
            compositeDisposable.Add(disposable);
        }
        
        void GrabStart(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab start: " + slotId);
            
            var inventory = inventoryBroker.GetInventory(slotId.InventoryId);
            
            var slot = inventory.GetSlot(slotId);
            if (slot.Item == StackItem.Empty)
            {
                return;
            }
            
            grabbingItem.Value = new GrabbingInventoryItem(slot.Item);
            inventory.SetSlot(slotId, Slot.Empty);
        }

        void GrabEnd(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab end: " + slotId);

            var inventory = inventoryBroker.GetInventory(slotId.InventoryId);
            
            var placeSlot = inventory.GetSlot(slotId);
            if (placeSlot.Item == StackItem.Empty)
            {
                inventory.SetSlot(slotId, new Slot(grabbingItem.Value.Item));
                grabbingItem.Value = GrabbingInventoryItem.Empty;
                return;
            }
            
            var (primaryItem, subItem) = grabbingItem.Value.Item.Merge(placeSlot.Item);
            inventory.SetSlot(slotId, new Slot(primaryItem));

            if (subItem == StackItem.Empty)
            {
                grabbingItem.Value = GrabbingInventoryItem.Empty;
            }
            else
            {
                grabbingItem.Value = new GrabbingInventoryItem(subItem);
            }
        }

        void IDisposable.Dispose()
        {
            compositeDisposable.Dispose();
            disposableDictionary.Clear();
        }
    }
}