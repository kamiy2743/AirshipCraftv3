using Domain;
using Domain.Chunks;
using UnityView.ChunkRendering.RenderingSurface;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    internal class ChunkBoundsFactory
    {
        // TODO 仮実装
        private ChunkRenderingSurfaceProvider chunkRenderingSurfaceProvider;

        internal ChunkBoundsFactory(ChunkRenderingSurfaceProvider chunkRenderingSurfaceProvider)
        {
            this.chunkRenderingSurfaceProvider = chunkRenderingSurfaceProvider;
        }

        internal ChunkBounds Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var ChunkBounds = new ChunkBounds(chunkGridCoordinate.ToPivotCoordinate());
            var ChunkRenderingSurface = chunkRenderingSurfaceProvider.GetRenderingSurface(chunkGridCoordinate);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        // TODO 仮実装
                        var blockRenderingSurface = ChunkRenderingSurface.GetBlockRenderingSurface(rc);

                        if (!blockRenderingSurface.hasRenderingSurface)
                        {
                            continue;
                        }

                        var blockBounds = BlockBounds.CreateCubeBounds(new float3(x, y, z));
                        ChunkBounds.SetBlockBoundsDirectly(rc, blockBounds);
                    }
                }
            }

            return ChunkBounds;
        }
    }
}