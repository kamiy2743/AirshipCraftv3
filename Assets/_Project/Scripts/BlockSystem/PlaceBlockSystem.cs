using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;
using Util;

namespace BlockSystem
{
    public class PlaceBlockSystem
    {
        public static PlaceBlockSystem Instance => _instance;
        private static PlaceBlockSystem _instance;

        private ChunkDataStore _chunkDataStore;
        private ChunkObjectPool _chunkObjectPool;
        private ChunkMeshCreator _chunkMeshCreator;

        internal PlaceBlockSystem(ChunkDataStore chunkDataStore, ChunkObjectPool chunkObjectPool, ChunkMeshCreator chunkMeshCreator)
        {
            _instance = this;
            _chunkDataStore = chunkDataStore;
            _chunkObjectPool = chunkObjectPool;
            _chunkMeshCreator = chunkMeshCreator;
        }

        public async UniTask PlaceBlock(BlockID blockID, Vector3 position, CancellationToken ct)
        {
            if (!BlockCoordinate.IsValid(position)) return;

            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            var updateChunkList = new List<ChunkCoordinate>(4);

            // 設置対象のブロックデータをセットする
            {
                var bc = new BlockCoordinate(position);
                var cc = ChunkCoordinate.FromBlockCoordinate(bc);
                var lc = LocalCoordinate.FromBlockCoordinate(bc);
                var chunkData = _chunkDataStore.GetChunkData(cc);
                chunkData.SetBlockData(lc, new BlockData(blockID, bc));

                updateChunkList.Add(cc);
            }

            // 設置したブロックの周囲のブロックの接地ブロック情報を削除する
            foreach (var surface in SurfaceNormalExt.List)
            {
                var aroundPosition = position + surface.ToVector3();
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
                var meshData = _chunkMeshCreator.CreateMeshData(ref chunkData.Blocks);
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
