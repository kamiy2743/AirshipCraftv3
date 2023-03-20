using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record GlobalInventorySlotId
    {
        readonly InventoryId inventoryId;
        readonly IInventorySlotId slotId;

        public GlobalInventorySlotId(InventoryId inventoryId, IInventorySlotId slotId)
        {
            Assert.IsTrue(inventoryId == slotId.InventoryId);
            this.inventoryId = inventoryId;
            this.slotId = slotId;
        }

        public override string ToString() => inventoryId.ToString() + " => " + slotId.ToString();
    }
}