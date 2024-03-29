namespace ACv3.Domain.Chunks
{
    public readonly struct Block
    {
        public readonly BlockType blockType;

        public Block(BlockType blockType)
        {
            this.blockType = blockType;
        }

        internal Block DeepCopy()
        {
            return new Block(blockType);
        }
    }
}