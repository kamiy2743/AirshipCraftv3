using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
{
    public class ChunkSurface
    {
        public readonly ChunkGridCoordinate chunkGridCoordinate;
        readonly Dictionary<RelativeCoordinate, BlockSurface> blockSurfaces = new();

        internal ChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
        }

        ChunkSurface(ChunkGridCoordinate chunkGridCoordinate, Dictionary<RelativeCoordinate, BlockSurface> blockSurfaces)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
            this.blockSurfaces = blockSurfaces;
        }

        internal BlockSurface GetBlockSurface(RelativeCoordinate relativeCoordinate)
        {
            if (blockSurfaces.TryGetValue(relativeCoordinate, out var result))
            {
                return result;
            }

            return BlockSurface.Empty;
        }

        internal void SetBlockSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockSurface blockSurface)
        {
            blockSurfaces[relativeCoordinate] = blockSurface;
        }

        public ChunkSurface DeepCopy()
        {
            var blockSurfacesCopy = new Dictionary<RelativeCoordinate, BlockSurface>(blockSurfaces.Count);
            foreach (var item in blockSurfaces)
            {
                blockSurfacesCopy[item.Key] = item.Value;
            }

            return new ChunkSurface(chunkGridCoordinate, blockSurfacesCopy);
        }
    }
}