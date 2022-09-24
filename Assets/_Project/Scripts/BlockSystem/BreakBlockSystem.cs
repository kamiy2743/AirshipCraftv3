using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public static class BreakBlockSystem
    {
        private static BlockDataUpdater _blockDataUpdater;

        internal static void StartInitial(BlockDataUpdater blockDataUpdater)
        {
            _blockDataUpdater = blockDataUpdater;
        }

        public static async UniTask BreakBlock(BlockCoordinate targetCoordinate, CancellationToken ct)
        {
            var updateBlockData = new BlockData(BlockID.Air, targetCoordinate);
            await _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
