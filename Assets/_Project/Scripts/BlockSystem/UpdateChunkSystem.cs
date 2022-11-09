using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BlockSystem
{
    /// <summary>
    /// プレイヤーの周りのチャンクオブジェクトを作成、破棄する
    /// </summary>
    internal class UpdateChunkSystem
    {
        private UpdateAroundPlayerTask currentTask;

        private ChunkObjectPool _chunkObjectPool;
        private ChunkDataStore _chunkDataStore;
        private ChunkMeshCreator _chunkMeshCreator;

        internal UpdateChunkSystem(Transform player, ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator, CancellationToken ct)
        {
            _chunkObjectPool = chunkObjectPool;
            _chunkDataStore = chunkDataStore;
            _chunkMeshCreator = chunkMeshCreator;

            ct.Register(() => currentTask?.CancelAsync());

            // 初回の更新
            Vector3Int lastPlayerChunk = GetPlayerChunk(player.position);
            UpdateAroundPlayer(lastPlayerChunk).Forget();

            // 毎フレーム監視
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    // プレイヤーチャンクが変化したら更新
                    var playerChunk = GetPlayerChunk(player.position);
                    if (playerChunk != lastPlayerChunk)
                    {
                        UpdateAroundPlayer(playerChunk).Forget();
                        lastPlayerChunk = playerChunk;
                    }

                    // 別スレッドで作成したメッシュデータからChunkObjectを作成
                    while (currentTask.CreateChunkObjectQueue.Count > 0)
                    {
                        if (currentTask.CreateChunkObjectQueue.TryDequeue(out var item))
                        {
                            var chunkObject = _chunkObjectPool.TakeChunkObject(item.Key);
                            chunkObject.SetMesh(item.Value);
                        }
                        else
                        {
                            throw new Exception("取得に失敗しました");
                        }
                    }
                });
        }

        /// <summary>
        /// プレイヤーがいるチャンクに変換
        /// </summary>
        private Vector3Int GetPlayerChunk(Vector3 playerPosition)
        {
            return new Vector3Int(
                (int)math.floor(playerPosition.x) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.y) >> ChunkData.ChunkBlockSideShift,
                (int)math.floor(playerPosition.z) >> ChunkData.ChunkBlockSideShift
            );
        }

        /// <summary>
        /// プレイヤーのいるチャンクを中心とする立方体を作成する
        /// </summary>
        private async UniTask UpdateAroundPlayer(Vector3Int pc)
        {
            var lastTask = currentTask;
            var selfTask = new UpdateAroundPlayerTask(pc, _chunkObjectPool, _chunkDataStore, _chunkMeshCreator);
            currentTask = selfTask;

            // タスク実行中であればキャンセルし、終了を待つ
            if (lastTask is not null && lastTask.InProgress)
            {
                await lastTask.CancelAsync();
            }

            // 最新のタスクであれば開始
            if (selfTask.ID == currentTask.ID)
            {
                currentTask.Start().Forget();
            }
        }

        private class UpdateAroundPlayerTask
        {
            internal Guid ID = Guid.NewGuid();
            internal ConcurrentQueue<KeyValuePair<ChunkCoordinate, ChunkMeshData>> CreateChunkObjectQueue = new ConcurrentQueue<KeyValuePair<ChunkCoordinate, ChunkMeshData>>();
            internal bool InProgress => isStarted && !isCompleted;

            private CancellationTokenSource cts = new CancellationTokenSource();
            private CancellationToken ct;
            private bool isStarted = false;
            private bool isCompleted = false;

            private Vector3Int pc;
            private ChunkObjectPool _chunkObjectPool;
            private ChunkDataStore _chunkDataStore;
            private ChunkMeshCreator _chunkMeshCreator;

            internal UpdateAroundPlayerTask(Vector3Int pc, ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator)
            {
                this.pc = pc;
                _chunkObjectPool = chunkObjectPool;
                _chunkDataStore = chunkDataStore;
                _chunkMeshCreator = chunkMeshCreator;
                ct = cts.Token;
            }

            internal async UniTask CancelAsync()
            {
                if (cts.IsCancellationRequested) return;

                cts.Cancel();
                cts.Dispose();
                if (!isCompleted)
                {
                    await UniTask.WaitUntil(() => isCompleted);
                }
            }

            internal async UniTask Start()
            {
                isStarted = true;

                // 範囲外チャンクの開放
                ReleaseOutRangeChunk();

                // 別スレッドに退避
                await UniTask.SwitchToThreadPool();

                // キューに作成チャンクを追加
                var createMeshDataQueue = SetupCreateMeshDataQueue();

                // メッシュ作成
                CreateMeshDataFromQueue(createMeshDataQueue);

                // メインスレッドに戻す
                await UniTask.SwitchToMainThread();

                isCompleted = true;
            }

            /// <summary> 読みこみ範囲外のチャンクオブジェクトを解放する </summary>
            private void ReleaseOutRangeChunk()
            {
                var createdChunks = _chunkObjectPool.ChunkObjects.Keys.ToArray();
                if (createdChunks.Length == 0) return;

                var job = new ReleaseOutRangeChunkJob();
                job.createdChunksCount = createdChunks.Length;
                job.playerChunk = pc;
                job.releaseChunks = new NativeList<ChunkCoordinate>(Allocator.TempJob);

                unsafe
                {
                    fixed (ChunkCoordinate* createdChunksFirst = &createdChunks[0])
                    {
                        job.createdChunksFirst = createdChunksFirst;
                        job.Schedule().Complete();
                    }
                }

                foreach (var cc in job.releaseChunks)
                {
                    _chunkObjectPool.ReleaseChunkObject(cc);
                }

                job.releaseChunks.Dispose();
            }

            private Queue<ChunkCoordinate> SetupCreateMeshDataQueue()
            {
                var createMeshDataQueue = new Queue<ChunkCoordinate>();

                // 作成済みチャンク
                var createdChunkHashSet = _chunkObjectPool.ChunkObjects.Keys.ToHashSet();

                var yStart = math.max(pc.y - World.LoadChunkRadius, ChunkCoordinate.Min.y);
                var yEnd = math.min(pc.y + World.LoadChunkRadius, ChunkCoordinate.Max.y);
                // 指定されたxzにあるチャンクを下から順に作成キューに追加する
                void EnqueueChunk(int x, int z)
                {
                    // 下から順に追加
                    for (int y = yStart; y <= yEnd; y++)
                    {
                        var cc = new ChunkCoordinate(x, y, z, true);
                        // 作成していなければ追加
                        if (!createdChunkHashSet.Contains(cc))
                        {
                            createMeshDataQueue.Enqueue(cc);
                        }
                    }
                }

                // 中心
                if ((pc.x >= ChunkCoordinate.Min.x && pc.x <= ChunkCoordinate.Max.x) &&
                    (pc.z >= ChunkCoordinate.Min.z && pc.z <= ChunkCoordinate.Max.z))
                {
                    EnqueueChunk(pc.x, pc.z);
                }

                // 内側から順に作成
                for (int r = 0; r <= World.LoadChunkRadius; r++)
                {
                    // 上から見てx+方向
                    if (pc.z + r <= ChunkCoordinate.Max.z)
                    {
                        var xe = math.min(pc.x + r - 1, ChunkCoordinate.Max.x);
                        for (int x = math.max(pc.x - r, ChunkCoordinate.Min.x); x <= xe; x++)
                        {
                            EnqueueChunk(x, pc.z + r);
                        }
                    }
                    // z-方向
                    if (pc.x + r <= ChunkCoordinate.Max.x)
                    {
                        var ze = math.max(pc.z - r + 1, ChunkCoordinate.Min.z);
                        for (int z = math.min(pc.z + r, ChunkCoordinate.Max.z); z >= ze; z--)
                        {
                            EnqueueChunk(pc.x + r, z);
                        }
                    }
                    // x-方向
                    if (pc.z - r >= ChunkCoordinate.Min.z)
                    {
                        var xe = math.max(pc.x - r + 1, ChunkCoordinate.Min.x);
                        for (int x = math.min(pc.x + r, ChunkCoordinate.Max.x); x >= xe; x--)
                        {
                            EnqueueChunk(x, pc.z - r);
                        }
                    }
                    // z+方向
                    if (pc.x - r >= ChunkCoordinate.Min.x)
                    {
                        var ze = math.min(pc.z + r - 1, ChunkCoordinate.Max.z);
                        for (int z = math.max(pc.z - r, ChunkCoordinate.Min.z); z <= ze; z++)
                        {
                            EnqueueChunk(pc.x - r, z);
                        }
                    }
                }

                return createMeshDataQueue;
            }

            /// <summary> キューにあるチャンクのメッシュを順次作成 </summary>
            private void CreateMeshDataFromQueue(Queue<ChunkCoordinate> createMeshDataQueue)
            {
                while (createMeshDataQueue.Count > 0)
                {
                    if (ct.IsCancellationRequested) return;

                    var cc = createMeshDataQueue.Dequeue();

                    var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                    if (chunkData is null) return;

                    var meshData = _chunkMeshCreator.CreateMeshData(chunkData, ct);
                    chunkData.ReferenceCounter.Release();

                    if (ct.IsCancellationRequested) return;
                    CreateChunkObjectQueue.Enqueue(new KeyValuePair<ChunkCoordinate, ChunkMeshData>(cc, meshData));
                }
            }

            [BurstCompile]
            private unsafe struct ReleaseOutRangeChunkJob : IJob
            {
                [NativeDisableUnsafePtrRestriction][ReadOnly] public ChunkCoordinate* createdChunksFirst;
                [ReadOnly] public int createdChunksCount;
                [ReadOnly] public Vector3Int playerChunk;
                public NativeList<ChunkCoordinate> releaseChunks;

                public void Execute()
                {
                    var pcx = playerChunk.x;
                    var pcy = playerChunk.y;
                    var pcz = playerChunk.z;
                    for (int i = 0; i < createdChunksCount; i++)
                    {
                        var cc = createdChunksFirst + i;

                        var x = cc->x - pcx;
                        if (x < 0) x = -x;
                        if (x > World.LoadChunkRadius)
                        {
                            releaseChunks.Add(*cc);
                            continue;
                        }

                        var y = cc->y - pcy;
                        if (y < 0) y = -y;
                        if (y > World.LoadChunkRadius)
                        {
                            releaseChunks.Add(*cc);
                            continue;
                        }

                        var z = cc->z - pcz;
                        if (z < 0) z = -z;
                        if (z > World.LoadChunkRadius)
                        {
                            releaseChunks.Add(*cc);
                            continue;
                        }
                    }
                }
            }
        }
    }
}