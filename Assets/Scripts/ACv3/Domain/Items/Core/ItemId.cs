namespace ACv3.Domain.Items
{
    public record ItemId
    {
        readonly string id;
        const string EmptyId = "Empty";

        ItemId(string id)
        {
            this.id = id;
        }

        public string RawString() => id == EmptyId ? "" : id;
        public override string ToString() => $"ItemId: {id}";

        public static ItemId Empty => new(EmptyId);
        public static ItemId Dirt => new("Dirt");
        public static ItemId Stone => new("Stone");
    }
}