using System.Threading;
using UnityEngine;
using DataObject.Block;

namespace BlockOperator
{
    public static class PlaceBlockSystem
    {
        private static BlockDataUpdater _blockDataUpdater;

        public static void StartInitial(BlockDataUpdater blockDataUpdater)
        {
            _blockDataUpdater = blockDataUpdater;
        }

        public static void PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var updateBlockData = new BlockData(blockID, new BlockCoordinate(position));
            _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
