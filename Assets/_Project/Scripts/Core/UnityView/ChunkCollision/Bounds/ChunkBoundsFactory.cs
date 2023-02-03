using Domain;
using Domain.Chunks;
using UnityView.Rendering.Chunks;
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
            var chunkBounds = new ChunkBounds(chunkGridCoordinate);
            var chunkSurface = chunkSurfaceProvider.GetChunkSurface(chunkGridCoordinate);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        // TODO 仮実装
                        var blockSurface = chunkSurface.GetBlockSurface(rc);

                        if (!blockSurface.hasRenderingSurface)
                        {
                            continue;
                        }

                        var blockBounds = BlockBounds.CreateCubeBounds(new float3(x, y, z));
                        chunkBounds.SetBlockBoundsDirectly(rc, blockBounds);
                    }
                }
            }

            return chunkBounds;
        }
    }
}