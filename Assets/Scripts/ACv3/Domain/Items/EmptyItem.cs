namespace ACv3.Domain.Items
{
    public record EmptyItem : IItem
    {
        public ItemID ItemID => new ItemID();
        public Amount MaxAmount => new Amount(0);
    }
}