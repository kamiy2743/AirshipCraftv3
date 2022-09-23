using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MasterData.Block;
using Cysharp.Threading.Tasks;

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
            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            if (!BlockCoordinate.IsValid(position)) return;

            var bc = new BlockCoordinate(position);
            var cc = ChunkCoordinate.FromBlockCoordinate(bc);
            var lc = LocalCoordinate.FromBlockCoordinate(bc);

            var chunkData = _chunkDataStore.GetChunkData(cc);
            chunkData.SetBlockData(lc, new BlockData(blockID, bc));

            if (!_chunkObjectPool.ChunkObjects.ContainsKey(cc)) return;
            
            var meshData = _chunkMeshCreator.CreateMeshData(ref chunkData.Blocks);
            var chunkObject = _chunkObjectPool.ChunkObjects[cc];

            // UnityApiを使う処理をするのでメインスレッドに戻す
            await UniTask.SwitchToMainThread(ct);
            
            chunkObject.SetMesh(meshData);
        }
    }
}
