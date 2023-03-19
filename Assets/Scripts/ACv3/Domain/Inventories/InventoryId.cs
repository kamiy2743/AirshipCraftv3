namespace ACv3.Domain.Inventories
{
    public record InventoryId
    {
        readonly string id;

        public InventoryId(string id)
        {
            this.id = id;
        }
    }
}