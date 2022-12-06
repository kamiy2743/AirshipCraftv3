using UnityEngine;
using System.Threading;
using DataObject.Block;
using DataObject.Chunk;
using Unity.Mathematics;

namespace DataStore
{
    public class BlockDataAccessor
    {
        private ChunkDataStore _chunkDataStore;

        public BlockDataAccessor(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        public BlockData GetBlockData(Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return BlockData.Empty;
            return GetBlockData(new BlockCoordinate(position), ct);
        }

        public BlockData GetBlockData(int3 position, CancellationToken ct) => GetBlockData(position.x, position.y, position.z, ct);
        public BlockData GetBlockData(int x, int y, int z, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(x, y, z)) return BlockData.Empty;
            return GetBlockData(new BlockCoordinate(x, y, z), ct);
        }

        public BlockData GetBlockData(BlockCoordinate bc, CancellationToken ct)
        {
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);
            var chunkData = _chunkDataStore.GetChunkData(cc, ct);
            if (chunkData is null) return BlockData.Empty;

            var blockData = chunkData.GetBlockData(lc);
            chunkData.ReferenceCounter.Release();
            return blockData;
        }
    }
}
