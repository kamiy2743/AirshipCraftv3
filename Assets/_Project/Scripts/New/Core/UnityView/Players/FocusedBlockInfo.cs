using UnityEngine;
using Domain;

namespace UnityView.Players
{
    internal record FocusedBlockInfo
    {
        internal readonly BlockTypeID blockTypeID;
        internal readonly Vector3 pivotCoordinate;

        internal FocusedBlockInfo(BlockTypeID blockTypeID, Vector3 pivotCoordinate)
        {
            this.blockTypeID = blockTypeID;
            this.pivotCoordinate = pivotCoordinate;
        }
    }
}