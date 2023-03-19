using System.Collections.Generic;
using ACv3.Domain.Inventories;

namespace ACv3.UseCase
{
    public class InventoryService
    {
        readonly Dictionary<InventoryId, IInventory> inventories = new();
        public bool IsOpened { get; private set; } = false;

        public void AddInventory(IInventory inventory) => inventories.Add(inventory.Id, inventory);
        public void RemoveInventory(InventoryId id) => inventories.Remove(id);

        public void Open(InventoryId id)
        {
            inventories[id].Open();
            IsOpened = true;
        }

        public void InventoryClose()
        {
            foreach (var inventory in inventories.Values)
            {
                inventory.Close();
            }
            
            IsOpened = false;
        }
    }
}