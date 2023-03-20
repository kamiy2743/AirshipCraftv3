namespace ACv3.Domain.Items
{
    public record EmptyItem : IItem
    {
        public ItemId ItemId => ItemId.Empty;
        public Amount MaxAmount => new Amount(0);
    }
}