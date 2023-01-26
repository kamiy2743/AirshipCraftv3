using Unity.Mathematics;
using Domain.Chunks;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal class BlockRenderingSurfaces
    {
        private readonly BlockRenderingSurface[] surfaces;

        internal BlockRenderingSurfaces()
        {
            surfaces = new BlockRenderingSurface[(int)math.pow(Chunk.BlockSide, 3)];
        }

        private BlockRenderingSurfaces(BlockRenderingSurface[] surfaces)
        {
            this.surfaces = surfaces;
        }

        internal BlockRenderingSurface GetBlockRenderingSurface(RelativeCoordinate relativeCoordinate)
        {
            return surfaces[RelativeCoordinateToIndex(relativeCoordinate)];
        }

        internal void SetBlockRenderingSurfaceDirectly(RelativeCoordinate relativeCoordinate, BlockRenderingSurface blockRenderingSurface)
        {
            surfaces[RelativeCoordinateToIndex(relativeCoordinate)] = blockRenderingSurface;
        }

        private int RelativeCoordinateToIndex(RelativeCoordinate relativeCoordinate)
        {
            return (relativeCoordinate.x << (Chunk.BlockSideShift * 2)) + (relativeCoordinate.y << Chunk.BlockSideShift) + relativeCoordinate.z;
        }

        internal BlockRenderingSurfaces DeepCopy()
        {
            var surfacesCopy = new BlockRenderingSurface[surfaces.Length];

            for (int i = 0; i < surfaces.Length; i++)
            {
                surfacesCopy[i] = surfaces[i];
            }

            return new BlockRenderingSurfaces(surfacesCopy);
        }
    }
}