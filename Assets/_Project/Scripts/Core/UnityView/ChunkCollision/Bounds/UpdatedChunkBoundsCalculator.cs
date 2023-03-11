using System.Collections.Generic;
using Domain;
using UnityView.Rendering.Chunks;

namespace UnityView.ChunkCollision
{
    class UpdatedChunkBoundsCalculator
    {
        // TODO 仮実装
        ChunkSurfaceProvider chunkSurfaceProvider;
        readonly ChunkBoundsFactory chunkBoundsFactory;

        internal UpdatedChunkBoundsCalculator(ChunkSurfaceProvider chunkSurfaceProvider, ChunkBoundsFactory chunkBoundsFactory)
        {
            this.chunkSurfaceProvider = chunkSurfaceProvider;
            this.chunkBoundsFactory = chunkBoundsFactory;
        }

        internal IEnumerable<ChunkBounds> Calculate(BlockGridCoordinate updateCoordinate)
        {
            // TODO 仮実装
            var updatedChunkBounds = new Dictionary<ChunkGridCoordinate, ChunkBounds>(4);

            var targetChunkGridCoordinate = ChunkGridCoordinate.Parse(updateCoordinate);
            updatedChunkBounds.Add(targetChunkGridCoordinate, chunkBoundsFactory.Create(targetChunkGridCoordinate));

            foreach (var direction in DirectionExt.Array)
            {
                // 隣接しているブロックの座標を取得
                if (!updateCoordinate.TryAdd(direction.ToInt3(), out var adjacentCoordinate))
                {
                    continue;
                }

                var adjacentChunkGridCoordinate = ChunkGridCoordinate.Parse(adjacentCoordinate);
                if (updatedChunkBounds.ContainsKey(adjacentChunkGridCoordinate))
                {
                    continue;
                }

                updatedChunkBounds.Add(adjacentChunkGridCoordinate, chunkBoundsFactory.Create(adjacentChunkGridCoordinate));
            }

            return updatedChunkBounds.Values;
        }
    }
}