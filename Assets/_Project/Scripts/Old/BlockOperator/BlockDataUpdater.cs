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
        private ChunkRendererPool _chunkRendererPool;
        private ChunkMeshCreator _chunkMeshCreator;

        private object syncObject = new object();

        public BlockDataUpdater(ChunkDataStore chunkDataStore, ChunkDataFileIO chunkDataFileIO, ChunkRendererPool chunkRendererPool, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkDataFileIO = chunkDataFileIO;
            _chunkRendererPool = chunkRendererPool;
            _chunkMeshCreator = chunkMeshCreator;
        }

        public void UpdateBlockData(BlockData updateBlock, CancellationToken ct)
        {
            lock (syncObject)
            {
                var updateChunks = new HashSet<ChunkData>();

                SetUpdateBlockData(updateBlock, updateChunks, ct);

                UpdateChunks(updateChunks, ct);
            }
        }

        public void UpdateBlockData(IEnumerable<BlockData> updateBlocks, CancellationToken ct)
        {
            lock (syncObject)
            {
                var updateChunks = new HashSet<ChunkData>();

                foreach (var block in updateBlocks)
                {
                    SetUpdateBlockData(block, updateChunks, ct);
                }

                UpdateChunks(updateChunks, ct);
            }
        }

        /// <summary> 
        /// 更新ブロックとその周囲のブロックをチャンクに反映 
        /// </summary>
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

            // 更新したブロックの周囲のブロックの接している面情報を削除する
            foreach (var surface in SurfaceNormalExt.Array)
            {
                var aroundPosition = updateBlock.BlockCoordinate.ToInt3() + surface.ToInt3();
                if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                var bc = new BlockCoordinate(aroundPosition);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData is null) return;

                var blockData = chunkData.GetBlockData(lc);

                // 空気は削除する意味がない
                if (blockData.ID == BlockID.Air) continue;

                // 接している面情報を削除する
                blockData.SetContactOtherBlockSurfaces(SurfaceNormal.Empty);
                chunkData.SetBlockData(lc, blockData);

                updateChunks.Add(chunkData);
            }
        }

        /// <summary> 
        /// 更新チャンクをファイルに反映し、メッシュを再計算 
        /// </summary>
        private void UpdateChunks(HashSet<ChunkData> updateChunks, CancellationToken ct)
        {
            foreach (var updateChunk in updateChunks)
            {
                updateChunk.InvokeBlockUpdateEvent();

                // ファイルを更新
                _chunkDataFileIO.AddOrUpdate(updateChunk);

                // メッシュがあれば更新
                if (_chunkRendererPool.ChunkRenderers.TryGetValue(updateChunk.ChunkCoordinate, out var chunkRenderer))
                {
                    var meshData = _chunkMeshCreator.CreateMeshData(updateChunk, ct);
                    chunkRenderer.SetMesh(meshData);
                    meshData?.Release();
                }

                updateChunk.ReferenceCounter.Release();
            }
        }
    }
}
