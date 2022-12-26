using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using DataObject.Chunk;
using DataStore;
using UnityEngine;

namespace ChunkConstruction
{
    public class ChunkColliderSystem : IDisposable
    {
        private ChunkDataStore _chunkDataStore;
        private ChunkMeshCreatorUtil _chunkMeshCreatorUtil;
        private ChunkCollider _chunkColliderPrefab;
        private Transform _chunkColliderParent;

        private const int MaxWorkingTime = 300;
        private const int UpdateInterval = 60;

        private CompositeDisposable disposals = new CompositeDisposable();

        private Dictionary<ChunkCoordinate, WorkingChunkCollider> workingChunkColliders = new Dictionary<ChunkCoordinate, WorkingChunkCollider>();
        private Queue<ChunkCollider> availableColliderQueue = new Queue<ChunkCollider>();

        public ChunkColliderSystem(PlayerChunkChangeDetector playerChunkChangeDetector, ChunkDataStore chunkDataStore, ChunkMeshCreatorUtil chunkMeshCreatorUtil, ChunkCollider chunkColliderPrefab, Transform chunkColliderParent)
        {
            _chunkDataStore = chunkDataStore;
            _chunkMeshCreatorUtil = chunkMeshCreatorUtil;
            _chunkColliderPrefab = chunkColliderPrefab;
            _chunkColliderParent = chunkColliderParent;

            playerChunkChangeDetector.OnDetect
                .Subscribe(playerChunk =>
                {
                    UpdateAroundPlayer(playerChunk);
                })
                .AddTo(disposals);

            Observable.EveryUpdate()
                .ThrottleFirstFrame(UpdateInterval)
                .Subscribe(_ =>
                {
                    UpdateAroundPlayer(playerChunkChangeDetector.PlayerChunk);
                    DecreaseWorkingTime();
                })
                .AddTo(disposals);
        }

        private void UpdateAroundPlayer(int3 playerChunk)
        {
            for (int x = playerChunk.x - World.SimulationRadius; x <= playerChunk.x + World.SimulationRadius; x++)
            {
                for (int y = playerChunk.y - World.SimulationRadius; y <= playerChunk.y + World.SimulationRadius; y++)
                {
                    for (int z = playerChunk.z - World.SimulationRadius; z <= playerChunk.z + World.SimulationRadius; z++)
                    {
                        if (!ChunkCoordinate.IsValid(x, y, z)) continue;

                        var cc = new ChunkCoordinate(x, y, z);

                        // 既にコライダーがあれば有効時間を最大に
                        if (workingChunkColliders.TryGetValue(cc, out var workingCollider))
                        {
                            workingCollider.SetWorkingTime(MaxWorkingTime);
                            continue;
                        }

                        // コライダー新規作成
                        // TODO ctを渡す
                        var chunkData = _chunkDataStore.GetChunkData(cc, default);

                        // 描画面計算
                        _chunkMeshCreatorUtil.CalcContactOtherBlockSurfaces(chunkData, default);

                        // 再利用できるコライダーがなければprefabから作成
                        if (!availableColliderQueue.TryDequeue(out var collider))
                        {
                            collider = MonoBehaviour.Instantiate(_chunkColliderPrefab, parent: _chunkColliderParent);
                        }
                        collider.SetActive(true);

                        // コライダー生成
                        collider.UpdateCollider(chunkData.Blocks);

                        // ブロックの更新に合わせてコライダーも更新
                        var disposal = chunkData.OnBlockUpdate
                             .Subscribe(_ =>
                             {
                                 collider.UpdateCollider(chunkData.Blocks);
                             })
                             .AddTo(disposals);

                        workingChunkColliders.Add(cc, new WorkingChunkCollider(collider, chunkData, disposal, MaxWorkingTime));
                    }
                }
            }
        }

        private void DecreaseWorkingTime()
        {
            var chunks = workingChunkColliders.Keys.ToList();
            foreach (var cc in chunks)
            {
                var workingCollider = workingChunkColliders[cc];
                var time = workingCollider.DecreaseWorkingTime(UpdateInterval);

                if (time <= 0)
                {
                    workingChunkColliders.Remove(cc);
                    workingCollider.OnUpdateBlockData.Dispose();
                    workingCollider.ChunkData.ReferenceCounter.Release();
                    workingCollider.Collider.SetActive(false);
                    availableColliderQueue.Enqueue(workingCollider.Collider);
                }
            }
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }

    internal class WorkingChunkCollider
    {
        internal readonly ChunkCollider Collider;
        internal readonly ChunkData ChunkData;
        internal readonly IDisposable OnUpdateBlockData;
        internal int WorkingTime { get; private set; }

        internal WorkingChunkCollider(ChunkCollider collider, ChunkData chunkData, IDisposable onUpdateBlockData, int workingTime)
        {
            Collider = collider;
            ChunkData = chunkData;
            OnUpdateBlockData = onUpdateBlockData;
            WorkingTime = workingTime;
        }

        internal void SetWorkingTime(int time)
        {
            WorkingTime = time;
        }

        internal int DecreaseWorkingTime(int amount)
        {
            WorkingTime -= amount;
            return WorkingTime;
        }
    }
}
