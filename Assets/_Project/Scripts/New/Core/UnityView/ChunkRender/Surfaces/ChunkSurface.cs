using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRender
{
    public class ChunkSurface
    {
        public readonly ChunkGridCoordinate chunkGridCoordinate;
        private Dictionary<RelativeCoordinate, BlockSurface> blockSurfaces = new Dictionary<RelativeCoordinate, BlockSurface>();

        internal ChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
        }

        private ChunkSurface(ChunkGridCoordinate chunkGridCoordinate, Dictionary<RelativeCoordinate, BlockSurface> blockSurfaces)
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

        internal void SetBlockSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockSurface blockRenderingSurface)
        {
            blockSurfaces[relativeCoordinate] = blockRenderingSurface;
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