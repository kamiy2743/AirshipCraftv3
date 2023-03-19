using System.Collections.Generic;
using ACv3.Domain.Inventories;

namespace ACv3.UseCase
{
    public class InventoryBroker
    {
        readonly Dictionary<InventoryId, IInventory> inventories = new();
        public IReadOnlyDictionary<InventoryId, IInventory> Inventories => inventories;

        public void AddInventory(IInventory inventory) => inventories.Add(inventory.Id, inventory);
        public IInventory GetInventory(InventoryId id) => inventories[id];
        public void RemoveInventory(InventoryId id) => inventories.Remove(id);
    }
}