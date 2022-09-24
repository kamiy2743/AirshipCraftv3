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

        private UpdateBlockSystem _updateBlockSystem;

        internal PlaceBlockSystem(UpdateBlockSystem updateBlockSystem)
        {
            _instance = this;
            _updateBlockSystem = updateBlockSystem;
        }

        public async UniTask PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            var updateBlockData = new BlockData(blockID, new BlockCoordinate(position));
            await _updateBlockSystem.UpdateBlockData(updateBlockData, ct);
        }
    }
}
