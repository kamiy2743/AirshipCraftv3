namespace Domain.Items
{
    internal interface IItem
    {
        ItemID GetItemID();
        Amount GetMaxAmount();
    }
}