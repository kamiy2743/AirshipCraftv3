using System.Collections.Generic;
using ACv3.Domain.Inventories;

namespace ACv3.UseCase
{
    public class InventoryService
    {
        readonly Dictionary<InventoryId, IInventory> inventories = new();

        public void AddInventory(IInventory inventory)
        {
            inventories.Add(inventory.Id, inventory);
        }

        public void RemoveInventory(InventoryId id)
        {
            inventories.Remove(id);
        }

        public void OpenRequest(InventoryId id)
        {
            inventories[id].OpenRequest();
        }

        public void CloseRequest(InventoryId id)
        {
            inventories[id].CloseRequest();
        }
    }
}