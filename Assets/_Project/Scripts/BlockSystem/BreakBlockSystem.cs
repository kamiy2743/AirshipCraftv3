using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public class BreakBlockSystem : MonoBehaviour
    {
        [SerializeField] private DropItem dropItemPrefab;

        public static BreakBlockSystem Instance { get; private set; }

        private BlockDataUpdater _blockDataUpdater;
        private ChunkDataStore _chunkDataStore;

        internal void StartInitial(BlockDataUpdater blockDataUpdater, ChunkDataStore chunkDataStore)
        {
            Instance = this;
            _blockDataUpdater = blockDataUpdater;
            _chunkDataStore = chunkDataStore;
        }

        public async UniTask BreakBlock(BlockCoordinate targetCoordinate, CancellationToken ct)
        {
            var cc = ChunkCoordinate.FromBlockCoordinate(targetCoordinate);
            var lc = LocalCoordinate.FromBlockCoordinate(targetCoordinate);
            var chunkData = _chunkDataStore.GetChunkData(cc, ct);
            var targetBlockData = chunkData.GetBlockData(lc);

            var updateBlockData = new BlockData(BlockID.Air, targetCoordinate);
            await _blockDataUpdater.UpdateBlockData(updateBlockData, ct);

            return;
            var meshData = MasterBlockDataStore.GetData(targetBlockData.ID).MeshData;
            var dropItem = Instantiate(dropItemPrefab);
            dropItem.SetMesh(meshData);
            dropItem.SetPosition(targetCoordinate.Center);
        }
    }
}
