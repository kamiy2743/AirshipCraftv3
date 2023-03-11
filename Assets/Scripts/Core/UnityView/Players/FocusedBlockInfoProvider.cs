using Domain;
using Domain.Chunks;
using UnityEngine;

namespace UnityView.Players
{
    public class FocusedBlockInfoProvider
    {
        readonly PlayerCamera playerCamera;
        readonly IChunkProvider chunkProvider;

        const float MaxFocusDistance = 5;

        internal FocusedBlockInfoProvider(PlayerCamera playerCamera, IChunkProvider chunkProvider)
        {
            this.playerCamera = playerCamera;
            this.chunkProvider = chunkProvider;
        }

        internal bool TryGetFocusedBlockInfo(out FocusedBlockInfo result)
        {
            if (!Physics.Raycast(playerCamera.Position, playerCamera.Forward, out var raycastHit, MaxFocusDistance))
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
            var block = chunkProvider.GetChunk(cgc).GetBlock(rc);

            result = new FocusedBlockInfo(block.blockType, bgc.ToPivotCoordinate(), raycastHit.point, raycastHit.normal);
            return true;
        }
    }
}