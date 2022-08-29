namespace BlockSystem
{
    public class BlockData
    {
        public BlockCoordinate BlockCoordinate { get; private set; }

        public BlockData(BlockCoordinate bc)
        {
            BlockCoordinate = bc;
        }
    }
}
