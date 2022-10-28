using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UniRx;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Util;
using MasterData.Block;

namespace BlockSystem
{
    /// <summary>
    /// チャンク用のメッシュを作成する
    /// </summary>
    internal class ChunkMeshCreator : IDisposable
    {
        private ChunkDataStore _chunkDataStore;
        private Dictionary<ChunkMeshData, IDisposable> allocatedMeshDataDictionary = new Dictionary<ChunkMeshData, IDisposable>();
        private ConcurrentQueue<ChunkMeshData> reusableMeshDataQueue = new ConcurrentQueue<ChunkMeshData>();

        internal ChunkMeshCreator(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        public void Dispose()
        {
            foreach (var item in allocatedMeshDataDictionary)
            {
                item.Key.Dispose();
                item.Value.Dispose();
            }
        }

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// キャンセルされた場合、頂点が0だった場合はnullを返す
        /// </summary>
        internal ChunkMeshData CreateMeshData(ChunkData chunkData, CancellationToken ct)
        {
            ChunkMeshData meshData;
            try
            {
                // 描画面計算
                CalcContactOtherBlockSurfaces(chunkData, ct);

                meshData = GetChunkMeshData();
                meshData.Init(chunkData, ct);
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }

            if (meshData.Vertices.Length == 0) return null;
            return meshData;
        }

        /// <summary>
        /// ChunkMeshDataをできるだけ再利用するようにして取得
        /// </summary>
        internal ChunkMeshData GetChunkMeshData()
        {
            lock (allocatedMeshDataDictionary)
            {
                ChunkMeshData meshData;
                if (reusableMeshDataQueue.Count == 0)
                {
                    meshData = new ChunkMeshData();
                }
                else
                {
                    // 再利用キューから取得
                    if (!reusableMeshDataQueue.TryDequeue(out meshData))
                    {
                        throw new Exception("ChunkMeshDataの取得に失敗しました");
                    }
                    // 購読を解除
                    allocatedMeshDataDictionary[meshData].Dispose();
                }

                // meshDataが解放されたら再利用キューに追加
                var disposal = meshData.OnReleased.Subscribe(_ => reusableMeshDataQueue.Enqueue(meshData));
                // 購読を登録
                allocatedMeshDataDictionary[meshData] = disposal;

                return meshData;
            }
        }

        /// <summary>
        /// チャンク内のすべてのブロックの描画面を計算する
        /// </summary>
        private void CalcContactOtherBlockSurfaces(ChunkData chunkData, CancellationToken ct)
        {
            /// <summary>
            /// 指定された面のChunkDataを取得する
            /// </summary>
            var targetChunkCoordinate = chunkData.ChunkCoordinate;
            ChunkData GetAroundChunkData(SurfaceNormal surface)
            {
                var surfaceVector = surface.ToVector3();
                var ccx = targetChunkCoordinate.x + (int)surfaceVector.x;
                var ccy = targetChunkCoordinate.y + (int)surfaceVector.y;
                var ccz = targetChunkCoordinate.z + (int)surfaceVector.z;

                if (!ChunkCoordinate.IsValid(ccx, ccy, ccz))
                {
                    return ChunkData.Empty;
                }

                var cc = new ChunkCoordinate(ccx, ccy, ccz, true);
                var aroundChunkData = _chunkDataStore.GetChunkData(cc, ct);

                ct.ThrowIfCancellationRequested();
                return aroundChunkData;
            }

            // それぞれの面のChunkDataを取得
            var rightChunk = GetAroundChunkData(SurfaceNormal.Right);
            var leftChunk = GetAroundChunkData(SurfaceNormal.Left);
            var topChunk = GetAroundChunkData(SurfaceNormal.Top);
            var bottomChunk = GetAroundChunkData(SurfaceNormal.Bottom);
            var forwardChunk = GetAroundChunkData(SurfaceNormal.Forward);
            var backChunk = GetAroundChunkData(SurfaceNormal.Back);

            unsafe
            {
                // それぞれの面のBlocksの先頭のポインタをJobに渡す
                fixed (BlockData*
                    centerChunkBlocksFirst = &chunkData.Blocks[0],
                    rightChunkBlocksFirst = &rightChunk.Blocks[0],
                    leftChunkBlocksFirst = &leftChunk.Blocks[0],
                    topChunkBlocksFirst = &topChunk.Blocks[0],
                    bottomChunkBlocksFirst = &bottomChunk.Blocks[0],
                    forwardChunkBlocksFirst = &forwardChunk.Blocks[0],
                    backChunkBlocksFirst = &backChunk.Blocks[0])
                fixed (SurfaceNormal* surfaceNormalsFirst = &SurfaceNormalExt.Array[0])
                {
                    var job = new CalcContactOtherBlockSurfacesJob
                    {
                        centerChunkBlocksFirst = centerChunkBlocksFirst,
                        rightChunkBlocksFirst = rightChunkBlocksFirst,
                        leftChunkBlocksFirst = leftChunkBlocksFirst,
                        topChunkBlocksFirst = topChunkBlocksFirst,
                        bottomChunkBlocksFirst = bottomChunkBlocksFirst,
                        forwardChunkBlocksFirst = forwardChunkBlocksFirst,
                        backChunkBlocksFirst = backChunkBlocksFirst,
                        surfaceNormalsFirst = surfaceNormalsFirst,
                        surfaceNormalsCount = SurfaceNormalExt.Array.Length
                    };

                    job.Schedule().Complete();
                }
            }

            // 参照を解放
            rightChunk.ReferenceCounter.Release();
            leftChunk.ReferenceCounter.Release();
            topChunk.ReferenceCounter.Release();
            bottomChunk.ReferenceCounter.Release();
            forwardChunk.ReferenceCounter.Release();
            backChunk.ReferenceCounter.Release();
        }

        [BurstCompile]
        unsafe private struct CalcContactOtherBlockSurfacesJob : IJob
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* centerChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* rightChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* leftChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* topChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* bottomChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* forwardChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* backChunkBlocksFirst;

            [NativeDisableUnsafePtrRestriction][ReadOnly] public SurfaceNormal* surfaceNormalsFirst;
            [ReadOnly] public int surfaceNormalsCount;

            public void Execute()
            {
                for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
                {
                    BlockData* targetBlockData = centerChunkBlocksFirst + i;
                    if (targetBlockData->IsRenderSkip) continue;
                    if (!targetBlockData->NeedToCalcContactSurfaces) continue;

                    var surfaces = SurfaceNormal.Zero;
                    var targetCoordinateVector = targetBlockData->BlockCoordinate.ToVector3();

                    for (int j = 0; j < surfaceNormalsCount; j++)
                    {
                        var surface = *(surfaceNormalsFirst + j);

                        var checkCoordinate = targetCoordinateVector + surface.ToVector3();
                        if (!BlockCoordinate.IsValid(checkCoordinate)) continue;

                        var bc = new BlockCoordinate(checkCoordinate);
                        var aroundBlockData = GetAroundBlockData(surface, bc);
                        if (aroundBlockData->ID == BlockID.Air) continue;

                        surfaces = surfaces.Add(surface);
                    }

                    targetBlockData->SetContactOtherBlockSurfaces(surfaces);
                }
            }

            private BlockData* GetAroundBlockData(SurfaceNormal surface, BlockCoordinate bc)
            {
                var lc = LocalCoordinate.FromBlockCoordinate(bc);

                BlockData* aroundBlocksFirst = centerChunkBlocksFirst;
                switch (surface)
                {
                    case SurfaceNormal.Right:
                        if (lc.x == 0)
                            aroundBlocksFirst = rightChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Left:
                        if (lc.x == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = leftChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Top:
                        if (lc.y == 0)
                            aroundBlocksFirst = topChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Bottom:
                        if (lc.y == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = bottomChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Forward:
                        if (lc.z == 0)
                            aroundBlocksFirst = forwardChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Back:
                        if (lc.z == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = backChunkBlocksFirst;
                        break;
                }

                return aroundBlocksFirst + ChunkData.ToIndex(lc);
            }
        }
    }
}
