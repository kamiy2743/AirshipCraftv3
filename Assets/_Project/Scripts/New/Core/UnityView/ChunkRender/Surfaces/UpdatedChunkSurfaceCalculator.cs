using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRender
{
    internal class UpdatedChunkSurfaceCalculator
    {
        private IChunkProvider chunkProvider;
        private ChunkSurfaceProvider chunkSurfaceProvider;

        internal UpdatedChunkSurfaceCalculator(IChunkProvider chunkProvider, ChunkSurfaceProvider chunkSurfaceProvider)
        {
            this.chunkProvider = chunkProvider;
            this.chunkSurfaceProvider = chunkSurfaceProvider;
        }

        internal IEnumerable<ChunkSurface> Calculate(BlockGridCoordinate updateCoordinate)
        {
            var updatedChunkSurfaces = new Dictionary<ChunkGridCoordinate, ChunkSurface>();
            var targetBlockSurface = new BlockSurface();
            var targetChunkGridCoordinate = ChunkGridCoordinate.Parse(updateCoordinate);
            var targetRelativeCoordinate = RelativeCoordinate.Parse(updateCoordinate);
            var targetBlockTypeID = chunkProvider.GetChunk(targetChunkGridCoordinate).GetBlock(targetRelativeCoordinate).blockTypeID;

            foreach (var surface in DirectionExt.Array)
            {
                // 隣接しているブロックの座標を取得
                if (!updateCoordinate.TryAdd(surface.ToInt3(), out var adjacentCoordinate))
                {
                    continue;
                }

                var adjacentChunkGridCoordinate = ChunkGridCoordinate.Parse(adjacentCoordinate);
                var adjacentRelativeCoordinate = RelativeCoordinate.Parse(adjacentCoordinate);
                var adjacentBlock = chunkProvider.GetChunk(adjacentChunkGridCoordinate).GetBlock(adjacentRelativeCoordinate);

                if (targetBlockTypeID != BlockTypeID.Air)
                {
                    // 対象ブロックの描画面に追加
                    if (adjacentBlock.blockTypeID == BlockTypeID.Air)
                    {
                        targetBlockSurface += surface;
                    }
                }

                // 隣接ブロックの描画面を更新
                var adjacentRenderingSurface = chunkSurfaceProvider.GetChunkSurface(adjacentChunkGridCoordinate);
                var currentSurface = adjacentRenderingSurface.GetBlockSurface(adjacentRelativeCoordinate);
                var targetSurface = surface.Flip();

                if (targetBlockTypeID == BlockTypeID.Air)
                {
                    if (!currentSurface.Contains(targetSurface))
                    {
                        adjacentRenderingSurface.SetBlockSurfaceDirectly(adjacentRelativeCoordinate, currentSurface + targetSurface);
                        updatedChunkSurfaces[adjacentChunkGridCoordinate] = adjacentRenderingSurface;
                    }
                }
                else
                {
                    if (currentSurface.Contains(targetSurface))
                    {
                        adjacentRenderingSurface.SetBlockSurfaceDirectly(adjacentRelativeCoordinate, currentSurface - targetSurface);
                        updatedChunkSurfaces[adjacentChunkGridCoordinate] = adjacentRenderingSurface;
                    }
                }
            }

            // 対象ブロックの描画面を更新
            {
                var renderingSurface = chunkSurfaceProvider.GetChunkSurface(targetChunkGridCoordinate);
                renderingSurface.SetBlockSurfaceDirectly(targetRelativeCoordinate, targetBlockSurface);

                updatedChunkSurfaces[targetChunkGridCoordinate] = renderingSurface;
            }

            return updatedChunkSurfaces.Values;
        }
    }
}