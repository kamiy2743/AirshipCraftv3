namespace ACv3.Domain.Items
{
    public record Dirt : IItem
    {
        ItemId IItem.ItemId => ItemId.Dirt;
        Amount IItem.MaxAmount => new(100);
    }
}