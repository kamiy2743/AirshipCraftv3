using UnityEngine;
using Domain;
using Domain.Chunks;

namespace UnityView.Players
{
    class FocusedBlockInfoProvider
    {
        readonly PlayerCamera _playerCamera;
        readonly IChunkProvider _chunkProvider;

        const float MaxFocusDistance = 5;

        internal FocusedBlockInfoProvider(PlayerCamera playerCamera, IChunkProvider chunkProvider)
        {
            _playerCamera = playerCamera;
            _chunkProvider = chunkProvider;
        }

        internal bool TryGetFocusedBlockInfo(out FocusedBlockInfo result)
        {
            if (!Physics.Raycast(_playerCamera.Position, _playerCamera.Forward, out var raycastHit, MaxFocusDistance))
            {
                result = null;
                return false;
            }

            if (!BlockGridCoordinate.TryParse(raycastHit.collider.bounds.center, out var bgc))
            {
                result = null;
                return false;
            }

            var cgc = ChunkGridCoordinate.Parse(bgc);
            var rc = RelativeCoordinate.Parse(bgc);
            var block = _chunkProvider.GetChunk(cgc).GetBlock(rc);

            result = new FocusedBlockInfo(block.BlockType, bgc.ToPivotCoordinate(), raycastHit.point, raycastHit.normal);
            return true;
        }
    }
}