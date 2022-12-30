using System.Threading;
using UnityEngine;
using DataObject.Block;

namespace BlockOperator
{
    public class PlaceBlockSystem
    {
        private BlockDataUpdater _blockDataUpdater;

        public PlaceBlockSystem(BlockDataUpdater blockDataUpdater)
        {
            _blockDataUpdater = blockDataUpdater;
        }

        public void PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var updateBlockData = new BlockData(blockID, new BlockCoordinate(position));
            _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
