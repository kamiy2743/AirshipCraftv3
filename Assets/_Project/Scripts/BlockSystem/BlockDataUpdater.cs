using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    internal class BlockDataUpdater
    {
        private ChunkDataStore _chunkDataStore;
        private ChunkObjectPool _chunkObjectPool;
        private ChunkMeshCreator _chunkMeshCreator;

        internal BlockDataUpdater(ChunkDataStore chunkDataStore, ChunkObjectPool chunkObjectPool, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkObjectPool = chunkObjectPool;
            _chunkMeshCreator = chunkMeshCreator;
        }

        internal async UniTask UpdateBlockData(BlockData updateBlockData, CancellationToken ct)
        {
            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            var updateChunkHashSet = new HashSet<ChunkData>(4);

            // 更新対象のブロックデータをセットする
            {
                var bc = updateBlockData.BlockCoordinate;
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData == null)
                {
                    await UniTask.SwitchToMainThread(ct);
                    return;
                }
                chunkData.SetBlockData(lc, updateBlockData);

                updateChunkHashSet.Add(chunkData);
            }

            // 更新したブロックの周囲のブロックの接地ブロック情報を削除する
            foreach (var surface in SurfaceNormalExt.Array)
            {
                var aroundPosition = updateBlockData.BlockCoordinate.ToVector3() + surface.ToVector3();
                if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                var bc = new BlockCoordinate(aroundPosition);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData == null)
                {
                    await UniTask.SwitchToMainThread(ct);
                    return;
                }
                var index = ChunkData.ToIndex(lc);

                // 空気は削除する意味がない
                if (chunkData.Blocks[index].ID == BlockID.Air) continue;

                chunkData.Blocks[index].SetContactOtherBlockSurfaces(SurfaceNormal.Empty);

                updateChunkHashSet.Add(chunkData);
            }

            // 更新チャンクのメッシュを再計算する
            var chunkMeshDic = new Dictionary<ChunkObject, ChunkMeshData>(updateChunkHashSet.Count);
            foreach (var updateChunk in updateChunkHashSet)
            {
                // 生成されていないチャンクであればスキップ
                if (!_chunkObjectPool.ChunkObjects.ContainsKey(updateChunk.ChunkCoordinate)) continue;

                var chunkObject = _chunkObjectPool.ChunkObjects[updateChunk.ChunkCoordinate];
                var meshData = _chunkMeshCreator.CreateMeshData(updateChunk, ct);
                if (meshData == null)
                {
                    await UniTask.SwitchToMainThread(ct);
                    return;
                }
                chunkMeshDic.Add(chunkObject, meshData);
            }

            // UnityApiを使う処理をするのでメインスレッドに戻す
            await UniTask.SwitchToMainThread(ct);

            // ChunkObjectにメッシュをセット
            foreach (var chunkMesh in chunkMeshDic)
            {
                var chunkObject = chunkMesh.Key;
                var meshData = chunkMesh.Value;
                chunkObject.SetMesh(meshData);
            }
        }
    }
}
