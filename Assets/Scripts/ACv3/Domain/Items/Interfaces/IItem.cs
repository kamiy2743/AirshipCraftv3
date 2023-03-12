namespace ACv3.Domain.Items
{
    public interface IItem
    {
        public ItemID ItemID { get; }
        public Amount MaxAmount { get; }
    }
}