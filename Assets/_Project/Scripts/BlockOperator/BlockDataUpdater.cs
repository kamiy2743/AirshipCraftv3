using System.Collections.Generic;
using System.Threading;
using Util;
using DataObject.Block;
using DataObject.Chunk;
using DataStore;
using ChunkConstruction;

namespace BlockOperator
{
    public class BlockDataUpdater
    {
        private ChunkDataStore _chunkDataStore;
        private ChunkDataFileIO _chunkDataFileIO;
        private ChunkObjectPool _chunkObjectPool;
        private ChunkMeshCreator _chunkMeshCreator;

        public BlockDataUpdater(ChunkDataStore chunkDataStore, ChunkDataFileIO chunkDataFileIO, ChunkObjectPool chunkObjectPool, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkDataFileIO = chunkDataFileIO;
            _chunkObjectPool = chunkObjectPool;
            _chunkMeshCreator = chunkMeshCreator;
        }

        internal void UpdateBlockData(BlockData updateBlock, CancellationToken ct)
        {
            var spinLock = new FastSpinLock();
            try
            {
                spinLock.Enter();

                var updateChunks = new HashSet<ChunkData>();

                SetUpdateBlockData(updateBlock, updateChunks, ct);

                UpdateChunks(updateChunks, ct);
            }
            finally
            {
                spinLock.Exit();
            }
        }

        internal void UpdateBlockData(IEnumerable<BlockData> updateBlocks, CancellationToken ct)
        {
            var spinLock = new FastSpinLock();
            try
            {
                spinLock.Enter();

                var updateChunks = new HashSet<ChunkData>();

                foreach (var block in updateBlocks)
                {
                    SetUpdateBlockData(block, updateChunks, ct);
                }

                UpdateChunks(updateChunks, ct);
            }
            finally
            {
                spinLock.Exit();
            }
        }

        /// <summary> 更新ブロックとその周囲のブロックをチャンクに反映 </summary>
        private void SetUpdateBlockData(BlockData updateBlock, HashSet<ChunkData> updateChunks, CancellationToken ct)
        {
            // 更新対象のブロックデータをセットする
            {
                var bc = updateBlock.BlockCoordinate;
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData is null) return;

                chunkData.SetBlockData(lc, updateBlock);

                updateChunks.Add(chunkData);
            }

            // 更新したブロックの周囲のブロックの接地ブロック情報を削除する
            foreach (var surface in SurfaceNormalExt.Array)
            {
                var aroundPosition = updateBlock.BlockCoordinate.ToInt3() + surface.ToInt3();
                if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                var bc = new BlockCoordinate(aroundPosition);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData is null) return;

                var index = ChunkData.ToIndex(lc);

                // 空気は削除する意味がない
                if (chunkData.Blocks[index].ID == BlockID.Air) continue;

                chunkData.Blocks[index].SetContactOtherBlockSurfaces(SurfaceNormal.Empty);

                updateChunks.Add(chunkData);
            }
        }

        /// <summary> 更新チャンクをファイルに反映し、メッシュを再計算 </summary>
        private void UpdateChunks(HashSet<ChunkData> updateChunks, CancellationToken ct)
        {
            foreach (var updateChunk in updateChunks)
            {
                // ファイルを更新
                _chunkDataFileIO.Update(updateChunk);

                // 生成されていなければスルー
                if (!_chunkObjectPool.ChunkObjects.TryGetValue(updateChunk.ChunkCoordinate, out var chunkObject))
                {
                    continue;
                }

                // 更新チャンクのメッシュを再計算する
                var meshData = _chunkMeshCreator.CreateMeshData(updateChunk, ct);
                // ChunkObjectにメッシュをセット
                chunkObject.SetMesh(meshData);

                meshData?.Release();
                updateChunk.ReferenceCounter.Release();
            }
        }
    }
}
