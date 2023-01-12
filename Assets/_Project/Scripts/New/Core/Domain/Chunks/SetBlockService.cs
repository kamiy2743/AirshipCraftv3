using System.Collections.Generic;

namespace Domain.Chunks
{
    // ブロックの更新時には周囲のチャンクのブロックを更新する場合があるので、整合性を確保するためにサービスとして記述
    public class SetBlockService
    {
        private IChunkRepository chunkRepository;
        private IChunkProvider chunkProvider;

        public SetBlockService(IChunkRepository chunkRepository, IChunkProvider chunkProvider)
        {
            this.chunkRepository = chunkRepository;
            this.chunkProvider = chunkProvider;
        }

        public void SetBlock(BlockGridCoordinate targetBlockGridCoordinate, Block target)
        {
            var updateChunks = new HashSet<Chunk>();
            var targetAdjacentSurfaces = new AdjacentSurfaces();

            foreach (var surface in AdjacentSurfaces.Array)
            {
                // 隣接しているブロックの座標を取得
                if (!BlockGridCoordinate.TryGetAdjacentCoordinate(
                    surface,
                    targetBlockGridCoordinate,
                    out var adjacentCoordinate))
                {
                    continue;
                }

                // 隣接ブロックを取得
                var cgc = ChunkGridCoordinate.Parse(adjacentCoordinate);
                var rc = RelativeCoordinate.Parse(adjacentCoordinate);
                var adjacentChunk = chunkProvider.GetChunk(cgc);
                var adjacentBlock = adjacentChunk.GetBlock(rc);

                // 対象ブロックの接面情報に追加
                if (adjacentBlock.blockTypeID != BlockTypeID.Air)
                {
                    targetAdjacentSurfaces.Add(surface);
                }

                // 隣接ブロックの接面情報を更新
                var newSurfaces = adjacentBlock.adjacentSurfaces.Remove(surface.Flip());
                adjacentBlock.SetAdjacentSurfacesDirectly(newSurfaces);

                adjacentChunk.SetBlockDirectly(rc, adjacentBlock);
                updateChunks.Add(adjacentChunk);
            }

            // 対象ブロックの接面情報を更新
            {
                target.SetAdjacentSurfacesDirectly(targetAdjacentSurfaces);

                var cgc = ChunkGridCoordinate.Parse(targetBlockGridCoordinate);
                var rc = RelativeCoordinate.Parse(targetBlockGridCoordinate);
                var chunk = chunkProvider.GetChunk(cgc);

                chunk.SetBlockDirectly(rc, target);
                updateChunks.Add(chunk);
            }

            // 更新チャンクを保存
            foreach (var chunk in updateChunks)
            {
                chunkRepository.Store(chunk);
            }
        }
    }
}