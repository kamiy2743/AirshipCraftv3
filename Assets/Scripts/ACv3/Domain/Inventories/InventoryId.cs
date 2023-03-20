namespace ACv3.Domain.Inventories
{
    public record InventoryId
    {
        readonly string id;

        InventoryId(string id)
        {
            this.id = id;
        }

        public override string ToString() => $"InventoryId: {id}";

        public static InventoryId PlayerInventory => new("PlayerInventory");
    }
}