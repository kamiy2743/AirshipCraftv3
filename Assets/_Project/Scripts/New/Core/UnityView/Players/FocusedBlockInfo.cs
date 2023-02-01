using UnityEngine;
using Domain;

namespace UnityView.Players
{
    internal record FocusedBlockInfo
    {
        internal readonly BlockTypeID blockTypeID;
        internal readonly Vector3 pivotCoordinate;
        internal readonly Vector3 hitPoint;
        internal readonly Vector3 hitNormal;

        internal FocusedBlockInfo(BlockTypeID blockTypeID, Vector3 pivotCoordinate, Vector3 hitPoint, Vector3 hitNormal)
        {
            this.blockTypeID = blockTypeID;
            this.pivotCoordinate = pivotCoordinate;
            this.hitPoint = hitPoint;
            this.hitNormal = hitNormal;
        }
    }
}