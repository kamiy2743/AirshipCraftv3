namespace ACv3.Domain.Inventories
{
    public record InventoryId
    {
        readonly string id;

        InventoryId(string id)
        {
            this.id = id;
        }

        public static InventoryId PlayerInventoryId => new("PlayerInventoryId");
    }
}