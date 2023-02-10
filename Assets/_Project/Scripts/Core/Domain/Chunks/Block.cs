namespace Domain.Chunks
{
    public readonly struct Block
    {
        public readonly BlockType BlockType;

        public Block(BlockType blockType)
        {
            BlockType = blockType;
        }

        internal Block DeepCopy()
        {
            return new Block(BlockType);
        }
    }
}