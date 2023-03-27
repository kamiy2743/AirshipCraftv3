namespace ACv3.Domain.Windows
{
    public record WindowId
    {
        readonly string id;

        WindowId(string id)
        {
            this.id = id;
        }
        
        public static WindowId PlayerWindow => new("PlayerWindow");
    }
}