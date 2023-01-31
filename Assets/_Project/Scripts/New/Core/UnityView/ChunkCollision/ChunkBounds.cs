using System.Collections.Generic;
using UnityEngine;
using Domain.Chunks;

namespace UnityView.ChunkCollision
{
    internal class ChunkBounds
    {
        internal readonly Vector3 pivotCoordinate;

        private Dictionary<RelativeCoordinate, BlockBounds> boundsDictionary = new Dictionary<RelativeCoordinate, BlockBounds>();
        internal IReadOnlyCollection<BlockBounds> BlockBoundsCollection => boundsDictionary.Values;

        internal ChunkBounds(Vector3 pivotCoordinate)
        {
            this.pivotCoordinate = pivotCoordinate;
        }

        internal void SetBlockBoundsDirectly(RelativeCoordinate relativeCoordinate, BlockBounds bounds)
        {
            boundsDictionary[relativeCoordinate] = bounds;
        }
    }
}