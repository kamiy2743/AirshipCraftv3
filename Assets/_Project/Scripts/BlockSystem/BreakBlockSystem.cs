using UnityEngine;
using System.Threading;
using MasterData.Block;

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

        public void BreakBlock(BlockData targetBlockData, CancellationToken ct)
        {
            var updateBlockData = new BlockData(BlockID.Air, targetBlockData.BlockCoordinate);
            _blockDataUpdater.UpdateBlockData(updateBlockData, ct);

            var dropItem = Instantiate(dropItemPrefab);
            var meshData = MasterBlockDataStore.GetData(targetBlockData.ID).MeshData;
            dropItem.SetMesh(meshData);
            dropItem.SetPosition(targetBlockData.BlockCoordinate.Center);
        }
    }
}
