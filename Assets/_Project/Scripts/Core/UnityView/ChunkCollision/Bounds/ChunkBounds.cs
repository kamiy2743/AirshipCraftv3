using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkCollision
{
    internal class ChunkBounds
    {
        internal readonly ChunkGridCoordinate chunkGridCoordinate;

        private Dictionary<RelativeCoordinate, BlockBounds> boundsDictionary = new Dictionary<RelativeCoordinate, BlockBounds>();
        internal IReadOnlyCollection<BlockBounds> BlockBoundsCollection => boundsDictionary.Values;

        internal ChunkBounds(ChunkGridCoordinate chunkGridCoordinate)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
        }

        internal void SetBlockBoundsDirectly(RelativeCoordinate relativeCoordinate, BlockBounds bounds)
        {
            boundsDictionary[relativeCoordinate] = bounds;
        }
    }
}