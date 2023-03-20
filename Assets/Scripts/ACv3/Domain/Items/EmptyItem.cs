namespace ACv3.Domain.Items
{
    public record EmptyItem : IItem
    {
        ItemId IItem.ItemId => ItemId.Empty;
        Amount IItem.MaxAmount => Amount.Empty();
    }
}