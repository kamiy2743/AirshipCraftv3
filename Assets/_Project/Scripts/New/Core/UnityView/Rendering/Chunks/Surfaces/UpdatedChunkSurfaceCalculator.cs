using System.Collections.Generic;
using Domain;
using Domain.Chunks;

namespace UnityView.Rendering.Chunks
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
            var updatedChunkSurfaces = new Dictionary<ChunkGridCoordinate, ChunkSurface>(4);
            var targetBlockSurface = new BlockSurface();
            var targetChunkGridCoordinate = ChunkGridCoordinate.Parse(updateCoordinate);
            var targetRelativeCoordinate = RelativeCoordinate.Parse(updateCoordinate);
            var targetBlockType = chunkProvider.GetChunk(targetChunkGridCoordinate).GetBlock(targetRelativeCoordinate).blockType;

            foreach (var direction in DirectionExt.Array)
            {
                // 隣接しているブロックの座標を取得
                if (!updateCoordinate.TryAdd(direction.ToInt3(), out var adjacentCoordinate))
                {
                    continue;
                }

                var adjacentChunkGridCoordinate = ChunkGridCoordinate.Parse(adjacentCoordinate);
                var adjacentRelativeCoordinate = RelativeCoordinate.Parse(adjacentCoordinate);
                var adjacentBlockType = chunkProvider.GetChunk(adjacentChunkGridCoordinate).GetBlock(adjacentRelativeCoordinate).blockType;

                if (targetBlockType != BlockType.Air)
                {
                    // 対象ブロックの描画面に追加
                    if (adjacentBlockType == BlockType.Air)
                    {
                        targetBlockSurface += direction;
                    }
                }

                if (adjacentBlockType == BlockType.Air)
                {
                    continue;
                }

                if (!updatedChunkSurfaces.TryGetValue(adjacentChunkGridCoordinate, out var adjacentChunkSurface))
                {
                    adjacentChunkSurface = chunkSurfaceProvider.GetChunkSurface(adjacentChunkGridCoordinate);
                }

                var currentSurface = adjacentChunkSurface.GetBlockSurface(adjacentRelativeCoordinate);
                var targetDirection = direction.Flip();

                if (targetBlockType == BlockType.Air)
                {
                    if (!currentSurface.Contains(targetDirection))
                    {
                        adjacentChunkSurface.SetBlockSurfaceDirectly(adjacentRelativeCoordinate, currentSurface + targetDirection);
                        updatedChunkSurfaces[adjacentChunkGridCoordinate] = adjacentChunkSurface;
                    }
                }
                else
                {
                    if (currentSurface.Contains(targetDirection))
                    {
                        adjacentChunkSurface.SetBlockSurfaceDirectly(adjacentRelativeCoordinate, currentSurface - targetDirection);
                        updatedChunkSurfaces[adjacentChunkGridCoordinate] = adjacentChunkSurface;
                    }
                }
            }

            // 対象ブロックの描画面を更新
            {
                if (!updatedChunkSurfaces.TryGetValue(targetChunkGridCoordinate, out var targetChunkSurface))
                {
                    targetChunkSurface = chunkSurfaceProvider.GetChunkSurface(targetChunkGridCoordinate);
                }

                targetChunkSurface.SetBlockSurfaceDirectly(targetRelativeCoordinate, targetBlockSurface);
                updatedChunkSurfaces[targetChunkGridCoordinate] = targetChunkSurface;
            }

            return updatedChunkSurfaces.Values;
        }
    }
}