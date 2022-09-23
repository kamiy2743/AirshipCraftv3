using System.Threading;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    internal class ChunkObjectCreator
    {
        private ChunkObjectPool _chunkObjectPool;
        private ChunkDataStore _chunkDataStore;
        private ChunkMeshCreator _chunkMeshCreator;

        internal ChunkObjectCreator(ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkObjectPool = chunkObjectPool;
            _chunkDataStore = chunkDataStore;
            _chunkMeshCreator = chunkMeshCreator;
        }

        /// <summary>
        /// 非同期かつ別スレッドでChunkObjectを作成します
        /// </summary>
        internal async UniTask<ChunkObject> CreateChunkObject(ChunkCoordinate cc, CancellationToken ct)
        {
            var (chunkObject, _) = await CreateChunkObject(cc, ct, null);
            return chunkObject;
        }

        /// <summary>
        /// 非同期かつ別スレッドでChunkObjectを作成します
        /// </summary>
        /// <param name="meshData">使いまわし用のChunkMeshData</param>
        internal async UniTask<(ChunkObject, ChunkMeshData)> CreateChunkObject(ChunkCoordinate cc, CancellationToken ct, ChunkMeshData meshData)
        {
            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            var chunkData = _chunkDataStore.GetChunkData(cc);
            var newMeshData = _chunkMeshCreator.CreateMeshData(ref chunkData.Blocks, meshData);

            // UnityApiを使う処理をするのでメインスレッドに戻す
            await UniTask.SwitchToMainThread(ct);

            var chunkObject = _chunkObjectPool.TakeChunkObject(cc);
            chunkObject.SetMesh(newMeshData);
            newMeshData.Clear();

            return (chunkObject, newMeshData);
        }
    }
}
