using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public class BreakBlockSystem
    {
        public static BreakBlockSystem Instance => _instance;
        private static BreakBlockSystem _instance;

        private BlockDataUpdater _blockDataUpdater;

        internal BreakBlockSystem(BlockDataUpdater blockDataUpdater)
        {
            _instance = this;
            _blockDataUpdater = blockDataUpdater;
        }

        public async UniTask BreakBlock(BlockCoordinate targetCoordinate, CancellationToken ct)
        {
            var updateBlockData = new BlockData(BlockID.Air, targetCoordinate);
            await _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
