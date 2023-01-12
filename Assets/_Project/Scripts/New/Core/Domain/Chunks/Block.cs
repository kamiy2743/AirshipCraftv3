namespace Domain.Chunks
{
    public class Block
    {
        internal readonly BlockTypeID blockTypeID;
        internal AdjacentSurfaces adjacentSurfaces { get; private set; }

        public Block(BlockTypeID blockTypeID)
        {
            this.blockTypeID = blockTypeID;
            adjacentSurfaces = new AdjacentSurfaces();
        }

        private Block(BlockTypeID blockTypeID, AdjacentSurfaces adjacentSurfaces)
        {
            this.blockTypeID = blockTypeID;
            this.adjacentSurfaces = adjacentSurfaces;
        }

        internal void SetAdjacentSurfacesDirectly(AdjacentSurfaces adjacentSurfaces)
        {
            this.adjacentSurfaces = adjacentSurfaces;
        }

        internal Block DeepCopy()
        {
            return new Block(blockTypeID, adjacentSurfaces);
        }
    }
}