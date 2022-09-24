using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public class PlaceBlockSystem
    {
        public static PlaceBlockSystem Instance => _instance;
        private static PlaceBlockSystem _instance;

        private BlockDataUpdater _blockDataUpdater;

        internal PlaceBlockSystem(BlockDataUpdater blockDataUpdater)
        {
            _instance = this;
            _blockDataUpdater = blockDataUpdater;
        }

        public async UniTask PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var updateBlockData = new BlockData(blockID, new BlockCoordinate(position));
            await _blockDataUpdater.UpdateBlockData(updateBlockData, ct);
        }
    }
}
