namespace ACv3.Domain.Items
{
    public record Stone : IItem
    {
        ItemId IItem.ItemId => ItemId.Stone;
        Amount IItem.MaxAmount => new(100);
    }
}