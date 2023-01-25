using System.Collections.Generic;
using Unity.Mathematics;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRendering.Model.RenderingSurface
{
    public class UpdateBlockRenderingSurfaceService
    {
        private IChunkProvider chunkProvider;
        private IChunkRenderingSurfaceRepository renderingSurfaceRepository;
        private IChunkRenderingSurfaceProvider renderingSurfaceProvider;

        internal UpdateBlockRenderingSurfaceService(IChunkProvider chunkProvider, IChunkRenderingSurfaceRepository renderingSurfaceRepository, IChunkRenderingSurfaceProvider renderingSurfaceProvider)
        {
            this.chunkProvider = chunkProvider;
            this.renderingSurfaceRepository = renderingSurfaceRepository;
            this.renderingSurfaceProvider = renderingSurfaceProvider;
        }

        public void UpdateBlockRenderingSurface(BlockGridCoordinate targetCoordinate, BlockTypeID targetBlockTypeID)
        {
            var updateRenderingSurfaces = new HashSet<ChunkRenderingSurface>();
            var targetBlockRenderingSurface = new BlockRenderingSurface();

            foreach (var surface in DirectionExt.Array)
            {
                // 隣接しているブロックの座標を取得
                if (!targetCoordinate.TryAdd(surface.ToInt3(), out var adjacentCoordinate))
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
                        updateRenderingSurfaces.Add(adjacentRenderingSurface);
                    }
                }
                else
                {
                    if (currentSurface.Contains(targetSurface))
                    {
                        adjacentRenderingSurface.SetBlockRenderingSurfaceDirectly(adjacentRelativeCoordinate, currentSurface - targetSurface);
                        updateRenderingSurfaces.Add(adjacentRenderingSurface);
                    }
                }
            }

            // 対象ブロックの描画面を更新
            {
                var cgc = ChunkGridCoordinate.Parse(targetCoordinate);
                var rc = RelativeCoordinate.Parse(targetCoordinate);
                var renderingSurface = renderingSurfaceProvider.GetRenderingSurface(cgc);
                renderingSurface.SetBlockRenderingSurfaceDirectly(rc, targetBlockRenderingSurface);

                updateRenderingSurfaces.Add(renderingSurface);
            }

            // 更新された描画面を保存
            foreach (var renderingSurface in updateRenderingSurfaces)
            {
                renderingSurfaceRepository.Store(renderingSurface);
            }
        }
    }
}