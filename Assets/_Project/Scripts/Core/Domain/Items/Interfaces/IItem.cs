namespace Domain.Items
{
    interface IItem
    {
        ItemID GetItemID();
        Amount GetMaxAmount();
    }
}