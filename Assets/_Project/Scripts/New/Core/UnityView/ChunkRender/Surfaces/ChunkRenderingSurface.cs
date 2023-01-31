using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRender.Surfaces
{
    public class ChunkRenderingSurface
    {
        public readonly ChunkGridCoordinate chunkGridCoordinate;
        private Dictionary<RelativeCoordinate, BlockRenderingSurface> blockSurfaces = new Dictionary<RelativeCoordinate, BlockRenderingSurface>();

        internal ChunkRenderingSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
        }

        private ChunkRenderingSurface(ChunkGridCoordinate chunkGridCoordinate, Dictionary<RelativeCoordinate, BlockRenderingSurface> blockSurfaces)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
            this.blockSurfaces = blockSurfaces;
        }

        internal BlockRenderingSurface GetBlockRenderingSurface(RelativeCoordinate relativeCoordinate)
        {
            if (blockSurfaces.TryGetValue(relativeCoordinate, out var result))
            {
                return result;
            }

            return BlockRenderingSurface.Empty;
        }

        internal void SetBlockRenderingSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockRenderingSurface blockRenderingSurface)
        {
            blockSurfaces[relativeCoordinate] = blockRenderingSurface;
        }

        public ChunkRenderingSurface DeepCopy()
        {
            var blockSurfacesCopy = new Dictionary<RelativeCoordinate, BlockRenderingSurface>(blockSurfaces.Count);
            foreach (var item in blockSurfaces)
            {
                blockSurfaces[item.Key] = item.Value;
            }

            return new ChunkRenderingSurface(chunkGridCoordinate, blockSurfacesCopy);
        }
    }
}