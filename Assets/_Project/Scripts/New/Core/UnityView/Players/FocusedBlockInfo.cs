using UnityEngine;
using Domain.Chunks;

namespace UnityView.Players
{
    internal record FocusedBlockInfo
    {
        internal readonly Block block;
        internal readonly Vector3 pivotCoordinate;

        internal FocusedBlockInfo(Block block, Vector3 pivotCoordinate)
        {
            this.block = block;
            this.pivotCoordinate = pivotCoordinate;
        }
    }
}