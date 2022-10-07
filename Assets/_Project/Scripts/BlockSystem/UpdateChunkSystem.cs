using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;

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

            ct.Register(() => currentTask.Cancel());

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
                        var item = currentTask.CreateChunkObjectQueue.Dequeue();
                        var chunkObject = _chunkObjectPool.TakeChunkObject(item.Key);
                        chunkObject.SetMesh(item.Value);
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
            if (lastTask != null && !lastTask.IsCompleted)
            {
                lastTask.Cancel();
                await UniTask.WaitUntil(() => lastTask.IsCompleted);
            }

            // 古いタスクであれば中断
            if (selfTask.ID != currentTask.ID)
            {
                return;
            }

            // タスク開始
            currentTask.Start().Forget();
        }

        private class UpdateAroundPlayerTask
        {
            private CancellationTokenSource cts = new CancellationTokenSource();
            private CancellationToken ct;

            private Vector3Int pc;
            private ChunkObjectPool _chunkObjectPool;
            private ChunkDataStore _chunkDataStore;
            private ChunkMeshCreator _chunkMeshCreator;

            private Queue<ChunkCoordinate> createChunkMeshDataQueue = new Queue<ChunkCoordinate>();
            internal Queue<KeyValuePair<ChunkCoordinate, ChunkMeshData>> CreateChunkObjectQueue = new Queue<KeyValuePair<ChunkCoordinate, ChunkMeshData>>();

            internal Guid ID = Guid.NewGuid();
            internal bool IsCompleted = false;

            internal UpdateAroundPlayerTask(Vector3Int pc, ChunkObjectPool chunkObjectPool, ChunkDataStore chunkDataStore, ChunkMeshCreator chunkMeshCreator)
            {
                this.pc = pc;
                _chunkObjectPool = chunkObjectPool;
                _chunkDataStore = chunkDataStore;
                _chunkMeshCreator = chunkMeshCreator;
                ct = cts.Token;
            }

            internal void Cancel()
            {
                cts.Cancel();
                cts.Dispose();
            }

            internal async UniTask Start()
            {
                // 読みこみ範囲外のチャンクオブジェクトを解放する
                foreach (var cc in _chunkObjectPool.ChunkObjects.Keys.ToList())
                {
                    if (math.abs(cc.x - pc.x) > World.LoadChunkRadius ||
                        math.abs(cc.y - pc.y) > World.LoadChunkRadius ||
                        math.abs(cc.z - pc.z) > World.LoadChunkRadius)
                    {
                        _chunkObjectPool.ReleaseChunkObject(cc);
                    }
                }

                // 別スレッドに退避
                await UniTask.SwitchToThreadPool();

                // キューに作成チャンクを追加
                SetupCreateMeshDataQueue();

                // メッシュ作成
                CreateMeshDataFromQueue();

                // メインスレッドに戻す
                await UniTask.SwitchToMainThread();

                IsCompleted = true;
            }

            private void SetupCreateMeshDataQueue()
            {
                // 作成済みチャンク
                var createdChunkHashSet = _chunkObjectPool.CreatedChunkHashSet;

                // 指定されたxzにあるチャンクを下から順に作成キューに追加する
                void EnqueueChunk(int x, int z)
                {
                    // 下から順に追加
                    var ye = math.min(pc.y + World.LoadChunkRadius, ChunkCoordinate.Max.y);
                    for (int y = math.max(pc.y - World.LoadChunkRadius, 0); y <= ye; y++)
                    {
                        var cc = new ChunkCoordinate(x, y, z, true);
                        // 作成済みならスキップ
                        if (createdChunkHashSet.Contains(cc)) continue;

                        createChunkMeshDataQueue.Enqueue(cc);
                    }
                }

                // 中心
                if ((pc.x >= ChunkCoordinate.Min.x && pc.x <= ChunkCoordinate.Max.x) &&
                    (pc.z >= ChunkCoordinate.Min.z && pc.z <= ChunkCoordinate.Max.z))
                {
                    EnqueueChunk(pc.x, pc.z);
                }

                // 内側から順に作成
                for (int r = 1; r <= World.LoadChunkRadius; r++)
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
            }

            /// <summary>
            /// キューにあるチャンクのメッシュを順次作成
            /// </summary>
            private void CreateMeshDataFromQueue()
            {
                while (createChunkMeshDataQueue.Count > 0)
                {
                    if (ct.IsCancellationRequested) return;

                    var cc = createChunkMeshDataQueue.Dequeue();

                    var chunkData = _chunkDataStore.GetChunkData(cc, ct);
                    if (chunkData == null) return;

                    var meshData = _chunkMeshCreator.CreateMeshData(chunkData, ct);
                    if (meshData == null) continue;

                    if (ct.IsCancellationRequested) return;

                    CreateChunkObjectQueue.Enqueue(new KeyValuePair<ChunkCoordinate, ChunkMeshData>(cc, meshData));
                }
            }
        }
    }
}