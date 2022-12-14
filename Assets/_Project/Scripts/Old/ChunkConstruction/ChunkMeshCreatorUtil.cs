using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Util;
using UniRx;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DataStore;
using MasterData.Block;
using DataObject.Block;
using DataObject.Chunk;

namespace ChunkConstruction
{
    public class ChunkMeshCreatorUtil
    {
        private ChunkDataStore _chunkDataStore;

        public ChunkMeshCreatorUtil(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        /// <summary> 
        /// チャンク内のすべてのブロックの描画面を計算する 
        /// </summary>
        internal void CalcContactOtherBlockSurfaces(ChunkData chunkData, CancellationToken ct)
        {
            // 指定された面のChunkDataを取得する
            var targetChunkCoordinate = chunkData.ChunkCoordinate;
            ChunkData GetAroundChunkData(SurfaceNormal surface)
            {
                var surfaceVector = surface.ToInt3();
                var ccx = targetChunkCoordinate.x + surfaceVector.x;
                var ccy = targetChunkCoordinate.y + surfaceVector.y;
                var ccz = targetChunkCoordinate.z + surfaceVector.z;

                if (!ChunkCoordinate.IsValid(ccx, ccy, ccz))
                {
                    return ChunkData.Empty;
                }

                var cc = new ChunkCoordinate(ccx, ccy, ccz, true);
                var aroundChunkData = _chunkDataStore.GetChunkData(cc, ct);

                return aroundChunkData;
            }

            // それぞれの面のChunkDataを取得
            var rightChunk = GetAroundChunkData(SurfaceNormal.Right);
            var leftChunk = GetAroundChunkData(SurfaceNormal.Left);
            var topChunk = GetAroundChunkData(SurfaceNormal.Top);
            var bottomChunk = GetAroundChunkData(SurfaceNormal.Bottom);
            var forwardChunk = GetAroundChunkData(SurfaceNormal.Forward);
            var backChunk = GetAroundChunkData(SurfaceNormal.Back);

            if (!ct.IsCancellationRequested)
            {
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
                            surfaceNormalsCount = SurfaceNormalExt.Array.Length
                        };

                        job.Schedule().Complete();
                    }
                }
            }

            // 参照を解放
            rightChunk?.ReferenceCounter.Release();
            leftChunk?.ReferenceCounter.Release();
            topChunk?.ReferenceCounter.Release();
            bottomChunk?.ReferenceCounter.Release();
            forwardChunk?.ReferenceCounter.Release();
            backChunk?.ReferenceCounter.Release();
        }

        [BurstCompile]
        private unsafe struct CalcContactOtherBlockSurfacesJob : IJob
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* centerChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* rightChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* leftChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* topChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* bottomChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* forwardChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* backChunkBlocksFirst;
            [ReadOnly] public int surfaceNormalsCount;

            public void Execute()
            {
                for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
                {
                    BlockData* targetBlockData = centerChunkBlocksFirst + i;
                    if (targetBlockData->IsRenderSkip) continue;
                    if (!targetBlockData->NeedToCalcContactSurfaces) continue;

                    var surfaces = SurfaceNormal.Zero;
                    var targetCoordinate = targetBlockData->BlockCoordinate.ToInt3();

                    for (int j = 0; j < surfaceNormalsCount; j++)
                    {
                        var surface = SurfaceNormalExt.FromIndex(j);
                        var surfaceVector = surface.ToInt3();

                        var bcx = targetCoordinate.x + surfaceVector.x;
                        var bcy = targetCoordinate.y + surfaceVector.y;
                        var bcz = targetCoordinate.z + surfaceVector.z;
                        if (!BlockCoordinate.IsValid(bcx, bcy, bcz)) continue;

                        var aroundBlockData = GetAroundBlockData(surface, bcx, bcy, bcz);
                        if (aroundBlockData->ID == BlockID.Air) continue;

                        surfaces = surfaces.Add(surface);
                    }

                    targetBlockData->SetContactOtherBlockSurfaces(surfaces);
                }
            }

            private BlockData* GetAroundBlockData(SurfaceNormal surface, int bcx, int bcy, int bcz)
            {
                var lcx = bcx & LocalCoordinate.ToLocalCoordinateMask;
                var lcy = bcy & LocalCoordinate.ToLocalCoordinateMask;
                var lcz = bcz & LocalCoordinate.ToLocalCoordinateMask;

                BlockData* aroundBlocksFirst = centerChunkBlocksFirst;
                switch (surface)
                {
                    case SurfaceNormal.Right:
                        if (lcx == 0)
                            aroundBlocksFirst = rightChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Left:
                        if (lcx == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = leftChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Top:
                        if (lcy == 0)
                            aroundBlocksFirst = topChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Bottom:
                        if (lcy == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = bottomChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Forward:
                        if (lcz == 0)
                            aroundBlocksFirst = forwardChunkBlocksFirst;
                        break;
                    case SurfaceNormal.Back:
                        if (lcz == ChunkData.ChunkBlockSide - 1)
                            aroundBlocksFirst = backChunkBlocksFirst;
                        break;
                }

                return aroundBlocksFirst + ChunkData.ToIndex(lcx, lcy, lcz);
            }
        }
    }
}
