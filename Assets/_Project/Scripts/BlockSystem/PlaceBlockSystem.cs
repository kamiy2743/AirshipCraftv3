using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    public class PlaceBlockSystem
    {
        private ChunkDataStore _chunkDataStore;
        private ChunkObjectPool _chunkObjectPool;
        private ChunkObjectCreator _chunkObjectCreator;

        internal PlaceBlockSystem(ChunkDataStore chunkDataStore, ChunkObjectPool chunkObjectPool, ChunkObjectCreator chunkObjectCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkObjectPool = chunkObjectPool;
            _chunkObjectCreator = chunkObjectCreator;
        }

        public async UniTask PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var bc = new BlockCoordinate(position);
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);

            var chunkData = _chunkDataStore.GetChunkData(cc);
            chunkData.SetBlockData(lc, new BlockData(blockID, bc));

            if (!_chunkObjectPool.ChunkObjects.ContainsKey(cc)) return;

            await _chunkObjectCreator.CreateChunkObject(cc, ct);
            _chunkObjectPool.ReleaseChunkObject(cc);
        }
    }
}
