using MasterData.Block;
using System.Threading;
using Util;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BlockSystem
{
    internal class ChunkMeshCreator
    {
        private ChunkDataStore _chunkDataStore;

        internal ChunkMeshCreator(ChunkDataStore chunkDataStore)
        {
            _chunkDataStore = chunkDataStore;
        }

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// </summary>
        internal ChunkMeshData CreateMeshData(ChunkData chunkData, CancellationToken ct)
        {
            try
            {
                CalcContactOtherBlockSurfaces(chunkData, ct);
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }

            int maxVerticesCount = 0;
            int maxTrianglesCount = 0;
            int maxUVsCount = 0;
            var masterBlockDataCache = MasterBlockDataStore.GetData(BlockID.Empty);
            foreach (var block in chunkData.Blocks)
            {
                if (block.IsRenderSkip) continue;

                if (block.ID != masterBlockDataCache.ID)
                {
                    masterBlockDataCache = MasterBlockDataStore.GetData(block.ID);
                }

                var blockMeshData = masterBlockDataCache.MeshData;
                maxVerticesCount += blockMeshData.Vertices.Length;
                maxTrianglesCount += blockMeshData.Triangles.Length;
                maxUVsCount += blockMeshData.UVs.Length;
            }

            var meshData = new ChunkMeshData(maxVerticesCount, maxTrianglesCount, maxUVsCount);
            if (ct.IsCancellationRequested) return null;

            foreach (var blockData in chunkData.Blocks)
            {
                meshData.AddBlock(blockData);
            }

            if (ct.IsCancellationRequested) return null;
            return meshData;
        }

        unsafe private void CalcContactOtherBlockSurfaces(ChunkData chunkData, CancellationToken ct)
        {
            var aroundChunkDataArray = new ChunkData[6];
            for (int i = 0; i < 6; i++)
            {
                var surfaceVector = SurfaceNormalExt.Array[i].ToVector3();
                var ccx = chunkData.ChunkCoordinate.x + (int)surfaceVector.x;
                var ccy = chunkData.ChunkCoordinate.y + (int)surfaceVector.y;
                var ccz = chunkData.ChunkCoordinate.z + (int)surfaceVector.z;

                if (!ChunkCoordinate.IsValid(ccx, ccy, ccz))
                {
                    aroundChunkDataArray[i] = ChunkData.Empty;
                    continue;
                }

                var cc = new ChunkCoordinate(ccx, ccy, ccz, true);
                var aroundChunkData = _chunkDataStore.GetChunkData(cc, ct);
                ct.ThrowIfCancellationRequested();

                aroundChunkDataArray[i] = aroundChunkData;
            }

            fixed (
                BlockData*
                centerChunkBlocksFirst = &chunkData.Blocks[0],
                rightChunkBlocksFirst = &aroundChunkDataArray[0].Blocks[0],
                leftChunkBlocksFirst = &aroundChunkDataArray[1].Blocks[0],
                topChunkBlocksFirst = &aroundChunkDataArray[2].Blocks[0],
                bottomChunkBlocksFirst = &aroundChunkDataArray[3].Blocks[0],
                forwardChunkBlocksFirst = &aroundChunkDataArray[4].Blocks[0],
                backChunkBlocksFirst = &aroundChunkDataArray[5].Blocks[0]
            )
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
                    surfaceNormalsFirst = surfaceNormalsFirst
                };

                job.Schedule(ChunkData.BlockCountInChunk, 0).Complete();
            }
        }

        [BurstCompile]
        unsafe private struct CalcContactOtherBlockSurfacesJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* centerChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* rightChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* leftChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* topChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* bottomChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* forwardChunkBlocksFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* backChunkBlocksFirst;

            [NativeDisableUnsafePtrRestriction][ReadOnly] public SurfaceNormal* surfaceNormalsFirst;

            public void Execute(int index)
            {
                BlockData* targetBlockData = centerChunkBlocksFirst + index;
                if (targetBlockData->IsRenderSkip) return;
                if (!targetBlockData->NeedToCalcContactSurfaces) return;

                var surfaces = SurfaceNormal.Zero;
                var targetCoordinateVector = targetBlockData->BlockCoordinate.ToVector3();

                for (int i = 0; i < 6; i++)
                {
                    var surface = *(surfaceNormalsFirst + i);

                    var checkCoordinate = targetCoordinateVector + surface.ToVector3();
                    if (!BlockCoordinate.IsValid(checkCoordinate)) continue;

                    var bc = new BlockCoordinate(checkCoordinate);
                    var aroundBlockData = GetAroundBlockData(surface, bc);

                    if (aroundBlockData->ID == BlockID.Air) continue;

                    surfaces = surfaces.Add(surface);
                }

                targetBlockData->SetContactOtherBlockSurfaces(surfaces);
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
