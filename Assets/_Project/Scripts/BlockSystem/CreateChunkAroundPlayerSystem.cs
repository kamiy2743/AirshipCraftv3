using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BlockSystem
{
    /// <summary> プレイヤーの周りのチャンクを作成する </summary>
    internal class CreateChunkAroundPlayerSystem : IDisposable
    {
        private ChunkObjectPool _chunkObjectPool;
        private ChunkDataStore _chunkDataStore;
        private ChunkMeshCreator _chunkMeshCreator;

        private IDisposable updateDisposal;

        private bool createChunkInProgress = false;
        private CancellationTokenSource cts;
        private NativeList<ChunkCoordinate> releaseChunkList = new NativeList<ChunkCoordinate>(Allocator.Persistent);
        private NativeQueue<ChunkCoordinate> createChunkQueue = new NativeQueue<ChunkCoordinate>(Allocator.Persistent);
        internal ConcurrentQueue<KeyValuePair<ChunkCoordinate, ChunkMeshData>> createChunkObjectQueue = new ConcurrentQueue<KeyValuePair<ChunkCoordinate, ChunkMeshData>>();

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            updateDisposal.Dispose();
            releaseChunkList.Dispose();
            createChunkQueue.Dispose();
        }

        internal CreateChunkAroundPlayerSystem(Transform player, ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator)
        {
            _chunkObjectPool = chunkObjectPool;
            _chunkDataStore = chunkDataStore;
            _chunkMeshCreator = chunkMeshCreator;

            // 初回の作成
            var lastPlayerChunk = GetPlayerChunk(player.position);
            CreateChunk(lastPlayerChunk).Forget();

            // 毎フレーム監視
            updateDisposal = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    // プレイヤーチャンクが変化したら範囲外のチャンクを解放
                    var playerChunk = GetPlayerChunk(player.position);
                    if (!playerChunk.Equals(lastPlayerChunk))
                    {
                        ReleaseOutRangeChunk(playerChunk);
                        lastPlayerChunk = playerChunk;
                    }

                    CreateChunk(playerChunk).Forget();

                    // 別スレッドで作成したメッシュデータからChunkObjectを作成
                    while (createChunkObjectQueue.TryDequeue(out var item))
                    {
                        var chunkObject = _chunkObjectPool.TakeChunkObject(item.Key);
                        chunkObject.SetMesh(item.Value);
                    }
                });
        }

        /// <summary> プレイヤーがいるチャンクに変換 </summary>
        private static int3 GetPlayerChunk(Vector3 playerPosition)
        {
            return new int3(
                (int)math.floor(playerPosition.x) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.y) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.z) >> ChunkData.ChunkBlockSideShift
            );
        }

        /// <summary> 読みこみ範囲外のチャンクオブジェクトを解放する </summary>
        private void ReleaseOutRangeChunk(int3 playerChunk)
        {
            var createdChunks = _chunkObjectPool.ChunkObjects.Keys.ToArray();
            if (createdChunks.Length == 0) return;

            releaseChunkList.Clear();

            var job = new ReleaseOutRangeChunkJob();
            job.createdChunksCount = createdChunks.Length;
            job.playerChunk = playerChunk;
            job.releaseChunkList = releaseChunkList;

            unsafe
            {
                fixed (ChunkCoordinate* createdChunksFirst = &createdChunks[0])
                {
                    job.createdChunksFirst = createdChunksFirst;
                    job.Schedule().Complete();
                }
            }

            foreach (var cc in job.releaseChunkList)
            {
                _chunkObjectPool.ReleaseChunkObject(cc);
            }
        }

        [BurstCompile]
        private unsafe struct ReleaseOutRangeChunkJob : IJob
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public ChunkCoordinate* createdChunksFirst;
            [ReadOnly] public int createdChunksCount;
            [ReadOnly] public int3 playerChunk;
            public NativeList<ChunkCoordinate> releaseChunkList;

            public void Execute()
            {
                var pcx = playerChunk.x;
                var pcy = playerChunk.y;
                var pcz = playerChunk.z;

                for (int i = 0; i < createdChunksCount; i++)
                {
                    var cc = createdChunksFirst + i;

                    if (Abs(cc->x - pcx) > World.LoadChunkRadius ||
                        Abs(cc->y - pcy) > World.LoadChunkRadius ||
                        Abs(cc->z - pcz) > World.LoadChunkRadius)
                    {
                        releaseChunkList.Add(*cc);
                    }
                }
            }

            private int Abs(int n)
            {
                return (n ^ (n >> 31)) - (n >> 31);
            }
        }

        private async UniTask CreateChunk(int3 playerChunk)
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }

            if (createChunkInProgress) return;

            createChunkInProgress = true;

            createChunkQueue.Clear();
            createChunkObjectQueue.Clear();
            cts = new CancellationTokenSource();

            // メインスレッドでしか取得できないからここで
            var camera = Camera.main;
            var viewportMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;

            // 別スレッドに退避
            await UniTask.SwitchToThreadPool();

            SetupCreateChunkQueue(playerChunk, viewportMatrix);

            CreateMeshDataFromQueue(cts.Token);

            // メインスレッドに戻す
            await UniTask.SwitchToMainThread();

            createChunkInProgress = false;
        }

        /// <summary> 作成対象のチャンクをキューに追加する </summary>
        private void SetupCreateChunkQueue(int3 playerChunk, float4x4 viewportMatrix)
        {
            var job = new SetupCreateChunkQueueJob
            {
                createdChunks = _chunkObjectPool.CreatedChunks,
                createMeshDataQueue = createChunkQueue,
                playerChunk = playerChunk,
                yStart = math.max(playerChunk.y - World.LoadChunkRadius, ChunkCoordinate.Min),
                yEnd = math.min(playerChunk.y + World.LoadChunkRadius, ChunkCoordinate.Max),
                viewportMatrix = viewportMatrix
            };

            job.Schedule().Complete();
        }

        [BurstCompile]
        private struct SetupCreateChunkQueueJob : IJob
        {
            [ReadOnly] public NativeParallelHashSet<ChunkCoordinate> createdChunks;
            public NativeQueue<ChunkCoordinate> createMeshDataQueue;

            [ReadOnly] public int3 playerChunk;
            [ReadOnly] public int yStart;
            [ReadOnly] public int yEnd;
            [ReadOnly] public float4x4 viewportMatrix;

            public void Execute()
            {
                var pcx = playerChunk.x;
                var pcy = playerChunk.y;
                var pcz = playerChunk.z;

                // 中心
                if ((pcx >= ChunkCoordinate.Min && pcx <= ChunkCoordinate.Max) &&
                    (pcz >= ChunkCoordinate.Min && pcz <= ChunkCoordinate.Max))
                {
                    EnqueueChunk(pcx, pcz);
                }

                // 内側から順に作成
                for (int r = 0; r <= World.LoadChunkRadius; r++)
                {
                    // 上から見てx+方向
                    if (pcz + r <= ChunkCoordinate.Max)
                    {
                        var xe = math.min(pcx + r - 1, ChunkCoordinate.Max);
                        for (int x = math.max(pcx - r, ChunkCoordinate.Min); x <= xe; x++)
                        {
                            EnqueueChunk(x, pcz + r);
                        }
                    }
                    // z-方向
                    if (pcx + r <= ChunkCoordinate.Max)
                    {
                        var ze = math.max(pcz - r + 1, ChunkCoordinate.Min);
                        for (int z = math.min(pcz + r, ChunkCoordinate.Max); z >= ze; z--)
                        {
                            EnqueueChunk(pcx + r, z);
                        }
                    }
                    // x-方向
                    if (pcz - r >= ChunkCoordinate.Min)
                    {
                        var xe = math.max(pcx - r + 1, ChunkCoordinate.Min);
                        for (int x = math.min(pcx + r, ChunkCoordinate.Max); x >= xe; x--)
                        {
                            EnqueueChunk(x, pcz - r);
                        }
                    }
                    // z+方向
                    if (pcx - r >= ChunkCoordinate.Min)
                    {
                        var ze = math.min(pcz + r - 1, ChunkCoordinate.Max);
                        for (int z = math.max(pcz - r, ChunkCoordinate.Min); z <= ze; z++)
                        {
                            EnqueueChunk(pcx - r, z);
                        }
                    }
                }
            }

            /// <summary> 指定されたxzにあるチャンクを下から順に作成キューに追加する </summary>
            private void EnqueueChunk(int x, int z)
            {
                // 下から順に追加
                for (int y = yStart; y <= yEnd; y++)
                {
                    // カメラの描画範囲に入っているか
                    if (!InCameraRange(x, y, z)) continue;

                    var cc = new ChunkCoordinate(x, y, z, true);
                    // 作成していなければ追加
                    if (!createdChunks.Contains(cc))
                    {
                        createMeshDataQueue.Enqueue(cc);
                    }
                }
            }

            private bool InCameraRange(int x, int y, int z)
            {
                if (InCameraRangeHelper(x, y, z)) return true;
                if (InCameraRangeHelper(x, y + 1, z)) return true;
                if (InCameraRangeHelper(x, y, z + 1)) return true;
                if (InCameraRangeHelper(x, y + 1, z + 1)) return true;
                if (InCameraRangeHelper(x + 1, y, z)) return true;
                if (InCameraRangeHelper(x + 1, y + 1, z)) return true;
                if (InCameraRangeHelper(x + 1, y, z + 1)) return true;
                if (InCameraRangeHelper(x + 1, y + 1, z + 1)) return true;
                return false;
            }

            private bool InCameraRangeHelper(int x, int y, int z)
            {
                var viewportPoint = WorldToViewportPoint(
                    x * ChunkData.ChunkBlockSide,
                    y * ChunkData.ChunkBlockSide,
                    z * ChunkData.ChunkBlockSide);

                if (viewportPoint.z > 1) return false;
                if (viewportPoint.x > 1 || viewportPoint.x < -1) return false;
                if (viewportPoint.y > 1 || viewportPoint.y < -1) return false;
                return true;
            }

            private float3 WorldToViewportPoint(int x, int y, int z)
            {
                var viewportPoint = MultiplyToViewportMatrix(x, y, z);
                var div = 1 / viewportPoint.w;
                return new float3(
                    viewportPoint.x * div,
                    viewportPoint.y * div,
                    viewportPoint.z * div
                );
            }

            private float4 MultiplyToViewportMatrix(float b1, float b2, float b3)
            {
                return new float4(
                    (viewportMatrix.c0.x * b1) + (viewportMatrix.c1.x * b2) + (viewportMatrix.c2.x * b3) + (viewportMatrix.c3.x),
                    (viewportMatrix.c0.y * b1) + (viewportMatrix.c1.y * b2) + (viewportMatrix.c2.y * b3) + (viewportMatrix.c3.y),
                    (viewportMatrix.c0.z * b1) + (viewportMatrix.c1.z * b2) + (viewportMatrix.c2.z * b3) + (viewportMatrix.c3.z),
                    (viewportMatrix.c0.w * b1) + (viewportMatrix.c1.w * b2) + (viewportMatrix.c2.w * b3) + (viewportMatrix.c3.w)
                );
            }
        }

        /// <summary> キューにあるチャンクのメッシュを順次作成 </summary>
        private void CreateMeshDataFromQueue(CancellationToken ct)
        {
            while (createChunkQueue.TryDequeue(out var cc))
            {
                var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                if (chunkData is null) return;

                var meshData = _chunkMeshCreator.CreateMeshData(chunkData, ct);
                chunkData.ReferenceCounter.Release();

                if (ct.IsCancellationRequested) return;
                createChunkObjectQueue.Enqueue(new KeyValuePair<ChunkCoordinate, ChunkMeshData>(cc, meshData));
            }
        }
    }
}
