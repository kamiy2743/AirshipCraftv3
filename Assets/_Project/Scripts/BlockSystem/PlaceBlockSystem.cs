using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public static class PlaceBlockSystem
    {
        private static BlockDataUpdater _blockDataUpdater;

        internal static void StartInitial(BlockDataUpdater blockDataUpdater)
        {
            _blockDataUpdater = blockDataUpdater;
        }

        public static async UniTask PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var updateBlockData = new BlockData(blockID, new BlockCoordinate(position));
            await _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
