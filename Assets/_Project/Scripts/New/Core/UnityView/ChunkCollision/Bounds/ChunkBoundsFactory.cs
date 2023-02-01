using Domain;
using Domain.Chunks;
using UnityView.ChunkRender;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    internal class ChunkBoundsFactory
    {
        // TODO 仮実装
        private ChunkSurfaceProvider chunkSurfaceProvider;

        internal ChunkBoundsFactory(ChunkSurfaceProvider chunkSurfaceProvider)
        {
            this.chunkSurfaceProvider = chunkSurfaceProvider;
        }

        internal ChunkBounds Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var ChunkBounds = new ChunkBounds(chunkGridCoordinate);
            var ChunkRenderingSurface = chunkSurfaceProvider.GetChunkSurface(chunkGridCoordinate);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        // TODO 仮実装
                        var blockRenderingSurface = ChunkRenderingSurface.GetBlockSurface(rc);

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