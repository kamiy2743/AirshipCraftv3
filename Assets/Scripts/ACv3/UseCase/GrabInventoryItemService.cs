using System;
using System.Collections.Generic;
using ACv3.Domain.Inventories;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UseCase
{
    public class GrabInventoryItemService : IInitializable, IDisposable
    {
        readonly InventoryBroker inventoryBroker;
        readonly Dictionary<InventoryId, IDisposable> disposableDictionary = new();
        readonly CompositeDisposable compositeDisposable = new();

        bool isGrabbing { get; set; } = false;

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
                    if (isGrabbing)
                    {
                        GrabEnd(slotId);
                    }
                    else
                    {
                        GrabStart(slotId);
                    }
                }); 
            disposableDictionary.Add(id, disposable);
            compositeDisposable.Add(disposable);
        }
        
        void GrabStart(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab start: " + slotId);
            isGrabbing = true;
        }

        void GrabEnd(GlobalInventorySlotId slotId)
        {
            Debug.Log("grab end: " + slotId);
            isGrabbing = false;
        }

        void IDisposable.Dispose()
        {
            compositeDisposable.Dispose();
            disposableDictionary.Clear();
        }
    }
}