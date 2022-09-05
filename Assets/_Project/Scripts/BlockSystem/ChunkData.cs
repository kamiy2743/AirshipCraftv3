namespace BlockSystem
{
    public class ChunkData
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }

        private BlockData[] blocks;

        public ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            blocks = new BlockData[WorldSettings.BlockCountInChunk];

            for (int x = 0; x < WorldSettings.LocalBlockSide; x++)
            {
                for (int y = 0; y < WorldSettings.LocalBlockSide; y++)
                {
                    for (int z = 0; z < WorldSettings.LocalBlockSide; z++)
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
            return (lc.y * WorldSettings.LocalBlockSide * WorldSettings.LocalBlockSide) + (lc.z * WorldSettings.LocalBlockSide) + lc.x;
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
