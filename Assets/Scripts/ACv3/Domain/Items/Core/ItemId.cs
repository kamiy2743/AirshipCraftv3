namespace ACv3.Domain.Items
{
    public record ItemId
    {
        readonly string id;

        ItemId(string id)
        {
            this.id = id;
        }

        public string DisplayString() => IsEmpty ? "" : id;
        public override string ToString() => $"ItemId: {id}";

        public static ItemId Empty => new("Empty");
        public bool IsEmpty => this == Empty;
        
        public static ItemId Dirt => new("Dirt");
        public static ItemId Stone => new("Stone");
    }
}