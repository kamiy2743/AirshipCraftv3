using UnityEngine;
using Domain;

namespace UnityView.Players
{
    record FocusedBlockInfo
    {
        internal readonly BlockType BlockType;
        internal readonly Vector3 PivotCoordinate;
        internal readonly Vector3 HitPoint;
        internal readonly Vector3 HitNormal;

        internal FocusedBlockInfo(BlockType blockType, Vector3 pivotCoordinate, Vector3 hitPoint, Vector3 hitNormal)
        {
            BlockType = blockType;
            PivotCoordinate = pivotCoordinate;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }
}