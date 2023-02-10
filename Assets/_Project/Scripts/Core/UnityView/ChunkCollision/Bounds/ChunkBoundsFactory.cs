using Domain;
using Domain.Chunks;
using UnityView.Rendering.Chunks;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    class ChunkBoundsFactory
    {
        // TODO 仮実装
        readonly ChunkSurfaceProvider _chunkSurfaceProvider;

        internal ChunkBoundsFactory(ChunkSurfaceProvider chunkSurfaceProvider)
        {
            _chunkSurfaceProvider = chunkSurfaceProvider;
        }

        internal ChunkBounds Create(ChunkGridCoordinate chunkGridCoordinate)
        {
            var chunkBounds = new ChunkBounds(chunkGridCoordinate);
            var chunkSurface = _chunkSurfaceProvider.GetChunkSurface(chunkGridCoordinate);

            for (int x = 0; x < Chunk.BlockSide; x++)
            {
                for (int y = 0; y < Chunk.BlockSide; y++)
                {
                    for (int z = 0; z < Chunk.BlockSide; z++)
                    {
                        var rc = new RelativeCoordinate(x, y, z);
                        // TODO 仮実装
                        var blockSurface = chunkSurface.GetBlockSurface(rc);

                        if (!blockSurface.HasRenderingSurface)
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