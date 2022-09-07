using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    public class UpdateChunkSystem
    {
        private ConcurrentQueue<ChunkCoordinate> createChunkQueue = new ConcurrentQueue<ChunkCoordinate>();
        private UniTask createChunkTask = UniTask.CompletedTask;

        private CancellationToken _cancellationToken;

        private ChunkDataStore _chunkDataStore;
        private ChunkObjectStore _chunkObjectStore;
        private ChunkMeshCreator _chunkMeshCreator;

        internal UpdateChunkSystem(Transform player, ChunkDataStore chunkDataStore, ChunkObjectStore chunkObjectStore, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkDataStore = chunkDataStore;
            _chunkObjectStore = chunkObjectStore;
            _chunkMeshCreator = chunkMeshCreator;


            ChunkCoordinate lastPlayerChunk = null;

            // 最初の一回
            CheckUpdate();
            // 毎フレーム監視
            Observable.EveryUpdate()
                .Subscribe(_ => CheckUpdate());

            void CheckUpdate()
            {
                if (!BlockCoordinate.IsValid(player.position)) return;
                var playerChunk = ChunkCoordinate.FromBlockCoordinate(new BlockCoordinate(player.position));

                if (playerChunk == lastPlayerChunk) return;
                lastPlayerChunk = playerChunk;

                UpdateAroundPlayer(playerChunk);
            }
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private void UpdateAroundPlayer(ChunkCoordinate pc)
        {
            var createdChunkList = _chunkDataStore.Chunks.Keys.ToList();

            // 中心
            EnqueueLoadChunkHelper(pc.x, pc.z, pc.y, createdChunkList);

            // 内側から順に作成
            for (int r = 1; r <= World.LoadChunkRadius; r++)
            {
                // 上から見てx+方向
                for (int x = pc.x - r; x < pc.x + r; x++)
                {
                    EnqueueLoadChunkHelper(x, pc.z + r, pc.y, createdChunkList);
                }
                // z-方向
                for (int z = pc.z + r; z > pc.z - r; z--)
                {
                    EnqueueLoadChunkHelper(pc.x + r, z, pc.y, createdChunkList);
                }
                // x-方向
                for (int x = pc.x + r; x > pc.x - r; x--)
                {
                    EnqueueLoadChunkHelper(x, pc.z - r, pc.y, createdChunkList);
                }
                // z+方向
                for (int z = pc.z - r; z < pc.z + r; z++)
                {
                    EnqueueLoadChunkHelper(pc.x - r, z, pc.y, createdChunkList);
                }
            }
        }

        /// <summary>
        /// 作成していないチャンクならキューに追加
        /// </summary>
        private void EnqueueLoadChunkHelper(int x, int z, int playerChunkY, IReadOnlyList<ChunkCoordinate> createdChunkList)
        {
            var radius = World.LoadChunkRadius;

            // 下から順に作成
            for (int y = playerChunkY - radius; y <= playerChunkY + radius; y++)
            {
                if (!ChunkCoordinate.IsValid(x, y, z)) continue;

                var cc = new ChunkCoordinate(x, y, z);
                if (createdChunkList.Contains(cc)) continue;

                createChunkQueue.Enqueue(cc);

                // タスクを開始していなければ開始
                if (createChunkTask.Equals(UniTask.CompletedTask))
                {
                    createChunkTask = CreateChunkFromQueue();
                    createChunkTask.Forget();
                }
            }
        }

        /// <summary>
        /// キューにあるチャンクを順次作成
        /// </summary>
        private async UniTask CreateChunkFromQueue()
        {
            while (createChunkQueue.Count > 0)
            {
                // 別スレッドに退避
                await UniTask.SwitchToThreadPool();

                if (!createChunkQueue.TryDequeue(out ChunkCoordinate cc))
                {
                    Debug.LogError("failed");
                    break;
                }

                var chunkData = _chunkDataStore.GetChunkData(cc);
                var meshData = _chunkMeshCreator.CreateMeshData(chunkData.Blocks);
                chunkData.SetMeshData(meshData);

                if (meshData.IsEmpty) continue;

                // UnityApiを使う処理をするのでメインスレッドに戻す
                await UniTask.SwitchToMainThread(_cancellationToken);

                _chunkObjectStore.CreateChunkObject(meshData.Mesh);
            }

            // タスク終了
            createChunkTask = UniTask.CompletedTask;
        }
    }
}