namespace ACv3.Domain.Items
{
    interface IItem
    {
        ItemID GetItemID();
        Amount GetMaxAmount();
    }
}