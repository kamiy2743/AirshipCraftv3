namespace ACv3.Domain.Inventories
{
    public record GlobalInventorySlotId
    {
        public readonly InventoryId InventoryId;
        public readonly int RawValue;

        public GlobalInventorySlotId(InventoryId inventoryId, int rawValue)
        {
            InventoryId = inventoryId;
            RawValue = rawValue;
        }

        public override string ToString() => $"{InventoryId} => RawValue: {RawValue}";
    }
}