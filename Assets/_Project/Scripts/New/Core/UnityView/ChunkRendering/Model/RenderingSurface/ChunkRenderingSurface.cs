using System;
using Domain.Chunks;

namespace UnityView.ChunkRendering.Model.RenderingSurface
{
    public class ChunkRenderingSurface : IEquatable<ChunkRenderingSurface>
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

        public override bool Equals(object obj)
        {
            return obj is ChunkRenderingSurface data && Equals(data);
        }

        public bool Equals(ChunkRenderingSurface other)
        {
            return this.chunkGridCoordinate == other.chunkGridCoordinate;
        }

        public override int GetHashCode()
        {
            return chunkGridCoordinate.GetHashCode();
        }
    }
}