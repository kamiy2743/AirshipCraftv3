using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkCollision
{
    class ChunkBounds
    {
        internal readonly ChunkGridCoordinate ChunkGridCoordinate;

        readonly Dictionary<RelativeCoordinate, BlockBounds> _boundsDictionary = new Dictionary<RelativeCoordinate, BlockBounds>();
        internal IReadOnlyCollection<BlockBounds> BlockBoundsCollection => _boundsDictionary.Values;

        internal ChunkBounds(ChunkGridCoordinate chunkGridCoordinate)
        {
            ChunkGridCoordinate = chunkGridCoordinate;
        }

        internal void SetBlockBoundsDirectly(RelativeCoordinate relativeCoordinate, BlockBounds bounds)
        {
            _boundsDictionary[relativeCoordinate] = bounds;
        }
    }
}