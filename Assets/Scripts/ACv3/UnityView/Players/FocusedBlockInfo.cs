using ACv3.Domain;
using UnityEngine;

namespace ACv3.UnityView.Players
{
    record FocusedBlockInfo
    {
        internal readonly BlockType blockType;
        internal readonly Vector3 pivotCoordinate;
        internal readonly Vector3 hitPoint;
        internal readonly Vector3 hitNormal;

        internal FocusedBlockInfo(BlockType blockType, Vector3 pivotCoordinate, Vector3 hitPoint, Vector3 hitNormal)
        {
            this.blockType = blockType;
            this.pivotCoordinate = pivotCoordinate;
            this.hitPoint = hitPoint;
            this.hitNormal = hitNormal;
        }
    }
}