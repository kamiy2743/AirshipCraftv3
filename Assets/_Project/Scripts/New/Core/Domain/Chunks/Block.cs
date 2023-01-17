namespace Domain.Chunks
{
    public class Block
    {
        internal readonly BlockTypeID blockTypeID;

        public Block(BlockTypeID blockTypeID)
        {
            this.blockTypeID = blockTypeID;
        }

        internal Block DeepCopy()
        {
            return new Block(blockTypeID);
        }
    }
}