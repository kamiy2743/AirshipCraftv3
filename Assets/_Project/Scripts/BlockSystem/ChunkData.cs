namespace BlockSystem
{
    public class ChunkData
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }

        private BlockData[] blocks;
        private int BlockSide => WorldSettings.LocalBlockSide;

        public ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            blocks = new BlockData[BlockSide * BlockSide * BlockSide];

            for (int x = 0; x < BlockSide; x++)
            {
                for (int y = 0; y < BlockSide; y++)
                {
                    for (int z = 0; z < BlockSide; z++)
                    {
                        var lc = new LocalCoordinate(x, y, z);
                        var bc = BlockCoordinate.FromChunkAndLocal(cc, lc);
                        var id = mapGenerator.GetBlockID(bc);
                        SetBlockData(lc, new BlockData(id, bc));
                    }
                }
            }
        }

        private int ToIndex(LocalCoordinate lc)
        {
            return (lc.y * BlockSide * BlockSide) + (lc.z * BlockSide) + lc.x;
        }

        private void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            blocks[ToIndex(lc)] = blockData;
        }

        public BlockData GetBlockData(LocalCoordinate lc)
        {
            return blocks[ToIndex(lc)];
        }
    }
}
