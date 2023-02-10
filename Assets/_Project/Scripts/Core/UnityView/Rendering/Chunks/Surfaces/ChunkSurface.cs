using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
{
    public class ChunkSurface
    {
        public readonly ChunkGridCoordinate ChunkGridCoordinate;
        readonly Dictionary<RelativeCoordinate, BlockSurface> _blockSurfaces = new Dictionary<RelativeCoordinate, BlockSurface>();

        internal ChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            ChunkGridCoordinate = chunkGridCoordinate;
        }

        ChunkSurface(ChunkGridCoordinate chunkGridCoordinate, Dictionary<RelativeCoordinate, BlockSurface> blockSurfaces)
        {
            ChunkGridCoordinate = chunkGridCoordinate;
            _blockSurfaces = blockSurfaces;
        }

        internal BlockSurface GetBlockSurface(RelativeCoordinate relativeCoordinate)
        {
            if (_blockSurfaces.TryGetValue(relativeCoordinate, out var result))
            {
                return result;
            }

            return BlockSurface.Empty;
        }

        internal void SetBlockSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockSurface blockSurface)
        {
            _blockSurfaces[relativeCoordinate] = blockSurface;
        }

        public ChunkSurface DeepCopy()
        {
            var blockSurfacesCopy = new Dictionary<RelativeCoordinate, BlockSurface>(_blockSurfaces.Count);
            foreach (var item in _blockSurfaces)
            {
                blockSurfacesCopy[item.Key] = item.Value;
            }

            return new ChunkSurface(ChunkGridCoordinate, blockSurfacesCopy);
        }
    }
}