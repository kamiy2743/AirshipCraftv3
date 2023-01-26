using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRendering.RenderingSurface
{
    public class ChunkRenderingSurface
    {
        public readonly ChunkGridCoordinate chunkGridCoordinate;
        private readonly BlockRenderingSurfaces surfaces;

        internal ChunkRenderingSurface(ChunkGridCoordinate chunkGridCoordinate, BlockRenderingSurfaces surfaces)
        {
            this.chunkGridCoordinate = chunkGridCoordinate;
            this.surfaces = surfaces;
        }

        internal BlockRenderingSurface GetBlockRenderingSurface(RelativeCoordinate relativeCoordinate)
        {
            return surfaces.GetBlockRenderingSurface(relativeCoordinate);
        }

        internal void SetBlockRenderingSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockRenderingSurface blockRenderingSurface)
        {
            surfaces.SetBlockRenderingSurfaceDirectly(relativeCoordinate, blockRenderingSurface);
        }

        public ChunkRenderingSurface DeepCopy()
        {
            return new ChunkRenderingSurface(chunkGridCoordinate, surfaces.DeepCopy());
        }
    }
}