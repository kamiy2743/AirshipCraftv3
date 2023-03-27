namespace ACv3.Domain.Inventories
{
    public record InventoryId
    {
        readonly string id;

        InventoryId(string id)
        {
            this.id = id;
        }

        public static InventoryId Empty => new("Empty");
        public bool IsEmpty => this == Empty;
        
        public override string ToString() => $"InventoryId: {id}";

        public static InventoryId PlayerInventory => new("PlayerInventory");
    }
}