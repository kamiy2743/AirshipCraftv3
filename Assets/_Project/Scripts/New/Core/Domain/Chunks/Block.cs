namespace Domain.Chunks
{
    public struct Block
    {
        public readonly BlockTypeID blockTypeID;

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