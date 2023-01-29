using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal class UpdatedChunkRenderingSurfaceCalculator
    {
        private IChunkProvider chunkProvider;
        private ChunkRenderingSurfaceProvider renderingSurfaceProvider;

        internal UpdatedChunkRenderingSurfaceCalculator(IChunkProvider chunkProvider, ChunkRenderingSurfaceProvider renderingSurfaceProvider)
        {
            this.chunkProvider = chunkProvider;
            this.renderingSurfaceProvider = renderingSurfaceProvider;
        }

        internal IEnumerable<ChunkRenderingSurface> Calculate(BlockGridCoordinate updateCoordinate)
        {
            var updatedRenderingSurfaces = new Dictionary<ChunkGridCoordinate, ChunkRenderingSurface>();
            var targetBlockRenderingSurface = new BlockRenderingSurface();
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
                        targetBlockRenderingSurface += surface;
                    }
                }

                // 隣接ブロックの描画面を更新
                var adjacentRenderingSurface = renderingSurfaceProvider.GetRenderingSurface(adjacentChunkGridCoordinate);
                var currentSurface = adjacentRenderingSurface.GetBlockRenderingSurface(adjacentRelativeCoordinate);
                var targetSurface = surface.Flip();

                if (targetBlockTypeID == BlockTypeID.Air)
                {
                    if (!currentSurface.Contains(targetSurface))
                    {
                        adjacentRenderingSurface.SetBlockRenderingSurfaceDirectly(adjacentRelativeCoordinate, currentSurface + targetSurface);
                        updatedRenderingSurfaces[adjacentChunkGridCoordinate] = adjacentRenderingSurface;
                    }
                }
                else
                {
                    if (currentSurface.Contains(targetSurface))
                    {
                        adjacentRenderingSurface.SetBlockRenderingSurfaceDirectly(adjacentRelativeCoordinate, currentSurface - targetSurface);
                        updatedRenderingSurfaces[adjacentChunkGridCoordinate] = adjacentRenderingSurface;
                    }
                }
            }

            // 対象ブロックの描画面を更新
            {
                var renderingSurface = renderingSurfaceProvider.GetRenderingSurface(targetChunkGridCoordinate);
                renderingSurface.SetBlockRenderingSurfaceDirectly(targetRelativeCoordinate, targetBlockRenderingSurface);

                updatedRenderingSurfaces[targetChunkGridCoordinate] = renderingSurface;
            }

            return updatedRenderingSurfaces.Values;
        }
    }
}