namespace ACv3.Domain.Items
{
    public interface IItem
    {
        public ItemId ItemId { get; }
        public Amount MaxAmount { get; }
    }
}