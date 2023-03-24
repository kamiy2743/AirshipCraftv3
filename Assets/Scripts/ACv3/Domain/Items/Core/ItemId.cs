namespace ACv3.Domain.Items
{
    public record ItemId
    {
        readonly string id;

        ItemId(string id)
        {
            this.id = id;
        }

        public string RawString() => id;
        public override string ToString() => $"ItemId: {id}";

        public static ItemId Empty => new("");
        public static ItemId Dirt => new("Dirt");
        public static ItemId Stone => new("Stone");
    }
}