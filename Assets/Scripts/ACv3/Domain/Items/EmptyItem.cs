namespace ACv3.Domain.Items
{
    public record EmptyItem : IItem
    {
        public ItemId ItemId => new ItemId();
        public Amount MaxAmount => new Amount(0);
    }
}