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

            var updateChunkList = new List<ChunkCoordinate>(4);

            // 更新対象のブロックデータをセットする
            {
                var bc = updateBlockData.BlockCoordinate;
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc);
                chunkData.SetBlockData(lc, updateBlockData);

                updateChunkList.Add(cc);
            }

            // 更新したブロックの周囲のブロックの接地ブロック情報を削除する
            foreach (var surface in SurfaceNormalExt.Array)
            {
                var aroundPosition = updateBlockData.BlockCoordinate.ToVector3() + surface.ToVector3();
                if (!BlockCoordinate.IsValid(aroundPosition)) continue;

                var bc = new BlockCoordinate(aroundPosition);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc);
                var index = ChunkData.ToIndex(lc);

                // 空気は削除する意味がない
                if (chunkData.Blocks[index].ID == BlockID.Air) continue;

                chunkData.Blocks[index].SetContactOtherBlockSurfaces(SurfaceNormal.Empty);

                if (!updateChunkList.Contains(cc))
                {
                    updateChunkList.Add(cc);
                }
            }

            // 更新チャンクのメッシュを再計算する
            var chunkMeshDic = new Dictionary<ChunkObject, ChunkMeshData>(updateChunkList.Count);
            foreach (var updateChunk in updateChunkList)
            {
                // 生成されていないチャンクであればスキップ
                if (!_chunkObjectPool.ChunkObjects.ContainsKey(updateChunk)) continue;

                var chunkObject = _chunkObjectPool.ChunkObjects[updateChunk];
                var chunkData = _chunkDataStore.GetChunkData(updateChunk);
                var meshData = _chunkMeshCreator.CreateMeshData(chunkData);
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
                meshData.Clear();
            }
        }
    }
}
