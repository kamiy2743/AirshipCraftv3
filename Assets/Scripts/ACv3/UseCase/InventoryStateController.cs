using ACv3.Domain.Inventories;
using Zenject;

namespace ACv3.UseCase
{
    public class InventoryStateController
    {
        readonly InventoryBroker inventoryBroker;
        public bool IsOpened { get; private set; } = false;

        [Inject]
        InventoryStateController(InventoryBroker inventoryBroker)
        {
            this.inventoryBroker = inventoryBroker;
        }

        public void Open(InventoryId id)
        {
            inventoryBroker.GetInventory(id).Open();
            IsOpened = true;
        }

        public void InventoryClose()
        {
            foreach (var inventory in inventoryBroker.Inventories.Values)
            {
                inventory.Close();
            }
            
            IsOpened = false;
        }
    }
}