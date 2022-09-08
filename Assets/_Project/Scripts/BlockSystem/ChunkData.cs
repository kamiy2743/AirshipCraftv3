using System.Collections.Generic;

namespace BlockSystem
{
    public class ChunkData
    {
        public ChunkCoordinate ChunkCoordinate { get; private set; }

        public IReadOnlyCollection<BlockData> Blocks => _blocks;
        private BlockData[] _blocks;

        public ChunkMeshData ChunkMeshData { get; private set; }

        public ChunkData(ChunkCoordinate cc, MapGenerator mapGenerator)
        {
            ChunkCoordinate = cc;
            _blocks = new BlockData[World.BlockCountInChunk];

            for (int x = 0; x < World.ChunkBlockSide; x++)
            {
                for (int y = 0; y < World.ChunkBlockSide; y++)
                {
                    for (int z = 0; z < World.ChunkBlockSide; z++)
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
            return (lc.y * World.ChunkBlockSide * World.ChunkBlockSide) + (lc.z * World.ChunkBlockSide) + lc.x;
        }

        private void SetBlockData(LocalCoordinate lc, BlockData blockData)
        {
            _blocks[ToIndex(lc)] = blockData;
        }

        public BlockData GetBlockData(LocalCoordinate lc)
        {
            return _blocks[ToIndex(lc)];
        }

        public void SetChunkMeshData(ChunkMeshData chunkMeshData)
        {
            ChunkMeshData = chunkMeshData;
        }
    }
}
