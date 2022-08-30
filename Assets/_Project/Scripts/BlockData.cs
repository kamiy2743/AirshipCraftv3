namespace BlockSystem
{
    public class BlockData
    {
        public int ID { get; private set; }
        public BlockCoordinate BlockCoordinate { get; private set; }

        public BlockData(int id, BlockCoordinate bc)
        {
            ID = id;
            BlockCoordinate = bc;
        }
    }
}
