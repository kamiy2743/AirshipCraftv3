using System.Collections.Generic;
using Domain;

namespace UnityView.ChunkCollision
{
    class UpdatedChunkBoundsCalculator
    {
        // TODO 仮実装
        readonly ChunkBoundsFactory _chunkBoundsFactory;

        internal UpdatedChunkBoundsCalculator(ChunkBoundsFactory chunkBoundsFactory)
        {
            _chunkBoundsFactory = chunkBoundsFactory;
        }

        internal IEnumerable<ChunkBounds> Calculate(BlockGridCoordinate updateCoordinate)
        {
            // TODO 仮実装
            var updatedChunkBounds = new Dictionary<ChunkGridCoordinate, ChunkBounds>(4);

            var targetChunkGridCoordinate = ChunkGridCoordinate.Parse(updateCoordinate);
            updatedChunkBounds.Add(targetChunkGridCoordinate, _chunkBoundsFactory.Create(targetChunkGridCoordinate));

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

                updatedChunkBounds.Add(adjacentChunkGridCoordinate, _chunkBoundsFactory.Create(adjacentChunkGridCoordinate));
            }

            return updatedChunkBounds.Values;
        }
    }
}