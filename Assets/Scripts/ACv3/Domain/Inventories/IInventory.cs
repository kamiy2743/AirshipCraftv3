namespace ACv3.Domain.Inventories
{
    public interface IInventory
    {
        InventoryId Id { get; }

        void Open();
        void Close();
    }
}