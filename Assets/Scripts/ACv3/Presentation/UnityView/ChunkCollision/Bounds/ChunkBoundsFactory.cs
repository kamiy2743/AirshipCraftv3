using ACv3.Domain;
using ACv3.Domain.Chunks;
using Unity.Mathematics;
using ACv3.Presentation.Rendering.Chunks;

namespace ACv3.Presentation.ChunkCollision
{
    public class ChunkBoundsFactory
    {
        // TODO 仮実装
        readonly ChunkSurfaceProvider chunkSurfaceProvider;

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