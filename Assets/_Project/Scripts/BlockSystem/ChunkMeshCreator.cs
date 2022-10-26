using MasterData.Block;
using System.Threading;
using Util;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace BlockSystem
{
    /// <summary>
    /// チャンク用のメッシュを作成する
    /// </summary>
    internal class ChunkMeshCreator
    {
        private ChunkDataStore _chunkDataStore;

        internal ChunkMeshCreator(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// キャンセルされた場合、頂点が0だった場合はnullを返す
        /// </summary>
        internal ChunkMeshData CreateMeshData(ChunkData chunkData, CancellationToken ct)
        {
            try
            {
                // 描画面計算
                CalcContactOtherBlockSurfaces(chunkData, ct);
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }

            // TODO BlockIDの個数を取得できるように
            /// <typeparam name="int">BlockID</typeparam>
            /// <typeparam name="int2-0">v,tの開始位置</typeparam>
            /// <typeparam name="int2-1">v,tのサイズ</typeparam>
            var masterMeshDataInfoHashMap = new NativeParallelHashMap<int, int2x2>(10, Allocator.TempJob);

            var masterVertices = new NativeList<Vector3>(Allocator.TempJob);
            var masterTriangles = new NativeList<int>(Allocator.TempJob);
            var masterUVs = new NativeList<Vector2>(Allocator.TempJob);

            void CleanUp()
            {
                masterMeshDataInfoHashMap.Dispose();
                masterVertices.Dispose();
                masterTriangles.Dispose();
                masterUVs.Dispose();
            }


            int maxVerticesCount = 0;
            int maxTrianglesCount = 0;
            var masterBlockDataCache = MasterBlockDataStore.GetData(BlockID.Empty);

            for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
            {
                if (chunkData.Blocks[i].IsRenderSkip) continue;

                var blockID = chunkData.Blocks[i].ID;
                if (blockID != masterBlockDataCache.ID)
                {
                    masterBlockDataCache = MasterBlockDataStore.GetData(blockID);
                }

                var masterMeshData = masterBlockDataCache.MeshData;
                maxVerticesCount += masterMeshData.Vertices.Length;
                maxTrianglesCount += masterMeshData.Triangles.Length;

                if (masterMeshDataInfoHashMap.ContainsKey((int)blockID)) continue;

                var masterMeshDataInfo = new int2x2(
                    masterVertices.Length, masterMeshData.Vertices.Length,
                    masterTriangles.Length, masterMeshData.Triangles.Length
                );
                masterMeshDataInfoHashMap.Add((int)blockID, masterMeshDataInfo);

                unsafe
                {
                    fixed (void*
                        vp = &masterMeshData.Vertices[0],
                        tp = &masterMeshData.Triangles[0],
                        up = &masterMeshData.UVs[0])
                    {
                        masterVertices.AddRange(vp, masterMeshData.Vertices.Length);
                        masterTriangles.AddRange(tp, masterMeshData.Triangles.Length);
                        masterUVs.AddRange(up, masterMeshData.Vertices.Length);
                    }
                }
            }

            if (ct.IsCancellationRequested || masterVertices.Length == 0)
            {
                CleanUp();
                return null;
            }

            ChunkMeshData meshData;
            unsafe
            {
                fixed (BlockData* blocksFirst = &chunkData.Blocks[0])
                {
                    meshData = new ChunkMeshData(
                        blocksFirst,
                        masterMeshDataInfoHashMap,
                        masterVertices,
                        masterTriangles,
                        masterUVs,
                        maxVerticesCount,
                        maxTrianglesCount);
                }
            }

            CleanUp();

            if (meshData.Vertices == null) return null;
            if (ct.IsCancellationRequested) return null;
            return meshData;
        }

        /// <summary>
        /// チャンク内のすべてのブロックの描画面を計算する
        /// </summary>
        unsafe private void CalcContactOtherBlockSurfaces(ChunkData chunkData, CancellationToken ct)
        {
            var cc = chunkData.ChunkCoordinate;
            fixed (BlockData*
                centerChunkBlocksFirst = &chunkData.Blocks[0],
                rightChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Right, ct).Blocks[0],
                leftChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Left, ct).Blocks[0],
                topChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Top, ct).Blocks[0],
                bottomChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Bottom, ct).Blocks[0],
                forwardChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Forward, ct).Blocks[0],
                backChunkBlocksFirst = &GetAroundChunkData(cc, SurfaceNormal.Back, ct).Blocks[0])
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

        /// <summary>
        /// 指定された面のChunkDataを取得する
        /// </summary>
        private ChunkData GetAroundChunkData(ChunkCoordinate targetChunkCoordinate, SurfaceNormal surface, CancellationToken ct)
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
