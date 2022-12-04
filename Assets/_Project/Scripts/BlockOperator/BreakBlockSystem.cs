using UnityEngine;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using MasterData.Block;
using DataObject.Block;
using DataStore;

namespace BlockOperator
{
    public class BreakBlockSystem : MonoBehaviour
    {
        [SerializeField] private DropItem dropItemPrefab;

        // TODO シングルトンをやめる
        public static BreakBlockSystem Instance { get; private set; }

        private BlockDataUpdater _blockDataUpdater;
        private ChunkDataStore _chunkDataStore;

        public void StartInitial(BlockDataUpdater blockDataUpdater, ChunkDataStore chunkDataStore)
        {
            Instance = this;
            _blockDataUpdater = blockDataUpdater;
            _chunkDataStore = chunkDataStore;
        }

        public void BreakBlock(BlockData targetBlock, CancellationToken ct)
        {
            var updateBlock = new BlockData(BlockID.Air, targetBlock.BlockCoordinate);
            _blockDataUpdater.UpdateBlockData(updateBlock, ct);

            CreateDropItem(targetBlock);
        }

        public void BreakBlock(IEnumerable<BlockData> targetBlocks, CancellationToken ct)
        {
            var updateBlocks = targetBlocks.Select(target => new BlockData(BlockID.Air, target.BlockCoordinate));
            _blockDataUpdater.UpdateBlockData(updateBlocks, ct);

            foreach (var block in targetBlocks)
            {
                CreateDropItem(block);
            }
        }

        private void CreateDropItem(BlockData blockData)
        {
            var dropItem = Instantiate(dropItemPrefab);
            var meshData = MasterBlockDataStore.GetData(blockData.ID).MeshData;
            dropItem.SetMesh(meshData);
            dropItem.SetPosition(blockData.BlockCoordinate.Center);
        }
    }
}
