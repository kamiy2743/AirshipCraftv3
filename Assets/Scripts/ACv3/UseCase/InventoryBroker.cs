using System;
using System.Collections.Generic;
using System.Linq;
using ACv3.Domain.Inventories;
using UniRx;

namespace ACv3.UseCase
{
    public class InventoryBroker
    {
        readonly ReactiveDictionary<InventoryId, IInventory> inventories = new();
        public IReadOnlyDictionary<InventoryId, IInventory> Inventories => inventories.ToDictionary(v => v.Key, v => v.Value);

        public void AddInventory(IInventory inventory) => inventories.Add(inventory.Id, inventory);
        public IInventory GetInventory(InventoryId id) => inventories[id];
        public void RemoveInventory(InventoryId id) => inventories.Remove(id);

        public IObservable<(InventoryId id, IInventory inventory)> ObserveAdd() => inventories.ObserveAdd().Select(e => (e.Key, e.Value));
        public IObservable<InventoryId> ObserveRemove() => inventories.ObserveRemove().Select(e => e.Key);
    }
}