using System.Collections.Generic;
using System.Threading;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;
using System.Linq;

namespace BlockSystem
{
    internal class BlockDataUpdater
    {
        private ChunkDataStore _chunkDataStore;
        private ChunkDataFileIO _chunkDataFileIO;
        private ChunkObjectPool _chunkObjectPool;
        private ChunkMeshCreator _chunkMeshCreator;

        internal BlockDataUpdater(ChunkDataStore chunkDataStore, ChunkDataFileIO chunkDataFileIO, ChunkObjectPool chunkObjectPool, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkDataFileIO = chunkDataFileIO;
            _chunkObjectPool = chunkObjectPool;
            _chunkMeshCreator = chunkMeshCreator;
        }

        internal bool UpdateBlockData(BlockData updateBlockData, CancellationToken ct)
        {
            lock (this)
            {
                var updateChunks = new HashSet<ChunkData>(5);

                // 更新対象のブロックデータをセットする
                {
                    var bc = updateBlockData.BlockCoordinate;
                    var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                    var lc = LocalCoordinate.FromBlockCoordinate(bc);
                    var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                    if (chunkData is null) return false;

                    chunkData.SetBlockData(lc, updateBlockData);

                    updateChunks.Add(chunkData);
                }

                // 更新したブロックの周囲のブロックの接地ブロック情報を削除する
                foreach (var surface in SurfaceNormalExt.Array)
                {
                    var aroundPosition = updateBlockData.BlockCoordinate.ToInt3() + surface.ToInt3();
                    if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                    var bc = new BlockCoordinate(aroundPosition);
                    var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                    var lc = LocalCoordinate.FromBlockCoordinate(bc);
                    var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                    if (chunkData is null) return false;

                    var index = ChunkData.ToIndex(lc);

                    // 空気は削除する意味がない
                    if (chunkData.Blocks[index].ID == BlockID.Air) continue;

                    chunkData.Blocks[index].SetContactOtherBlockSurfaces(SurfaceNormal.Empty);

                    updateChunks.Add(chunkData);
                }

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
                }

                return true;
            }
        }
    }
}
