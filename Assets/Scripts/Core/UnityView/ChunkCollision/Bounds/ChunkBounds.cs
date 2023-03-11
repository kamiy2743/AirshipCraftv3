using System.Collections.Generic;
using ACv3.Domain;
using ACv3.Domain.Chunks;

namespace ACv3.UnityView.ChunkCollision
{
    class ChunkBounds
    {
        internal readonly ChunkGridCoordinate chunkGridCoordinate;

        readonly Dictionary<RelativeCoordinate, BlockBounds> boundsDictionary = new();
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