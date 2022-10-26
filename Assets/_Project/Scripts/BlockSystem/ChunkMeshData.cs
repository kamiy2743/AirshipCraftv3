using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Util;

namespace BlockSystem
{
    internal class ChunkMeshData
    {
        internal Vector3[] Vertices;
        internal int[] Triangles;
        internal Vector2[] UVs;

        // TODO リファクタ
        unsafe internal ChunkMeshData(
            BlockData* blocksFirst,
            NativeParallelHashMap<int, int2x2> masterMeshDataInfoHashMap,
            NativeList<Vector3> masterVertices,
            NativeList<int> masterTriangles,
            NativeList<Vector2> masterUVs,
            int maxVerticesCount,
            int maxTrianglesCount)
        {
            var job = new ChunkMeshDataJob
            {
                blocksFirst = blocksFirst,
                masterMeshDataInfoHashMap = masterMeshDataInfoHashMap,
                masterVertices = masterVertices,
                masterTriangles = masterTriangles,
                masterUVs = masterUVs,
                resultVertices = new NativeList<Vector3>(maxVerticesCount, Allocator.TempJob),
                resultTriangles = new NativeList<int>(maxTrianglesCount, Allocator.TempJob),
                resultUVs = new NativeList<Vector2>(maxVerticesCount, Allocator.TempJob)
            };

            job.Schedule().Complete();

            if (job.resultVertices.Length > 0)
            {
                Vertices = job.resultVertices.ToArray();
                Triangles = job.resultTriangles.ToArray();
                UVs = job.resultUVs.ToArray();
            }

            job.resultVertices.Dispose();
            job.resultTriangles.Dispose();
            job.resultUVs.Dispose();
        }

        [BurstCompile]
        unsafe private struct ChunkMeshDataJob : IJob
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* blocksFirst;
            [ReadOnly] public NativeParallelHashMap<int, int2x2> masterMeshDataInfoHashMap;

            [ReadOnly] public NativeList<Vector3> masterVertices;
            [ReadOnly] public NativeList<int> masterTriangles;
            [ReadOnly] public NativeList<Vector2> masterUVs;

            public NativeList<Vector3> resultVertices;
            public NativeList<int> resultTriangles;
            public NativeList<Vector2> resultUVs;

            public void Execute()
            {
                for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
                {
                    var blockData = blocksFirst + i;
                    if (blockData->IsRenderSkip) continue;

                    var blockID = (int)blockData->ID;
                    var meshDataInfo = masterMeshDataInfoHashMap[blockID];
                    var meshStartIndex = meshDataInfo.c0;
                    var meshSize = meshDataInfo.c1;

                    // triangleはインデックスのため、現在の頂点数を加算しないといけない
                    var vc = resultVertices.Length;

                    // CubeMeshじゃないと動かない
                    for (int j = 0; j < 6; j++)
                    {
                        // 他のブロックに面していれば描画しない
                        if (blockData->IsContactOtherBlock(SurfaceNormalExt.FromIndex(j))) continue;

                        for (int k = 0; k < 6; k++)
                        {
                            var t = masterTriangles[meshStartIndex[1] + (j * 6 + k)];
                            resultTriangles.AddNoResize(t + vc);
                        }
                    }

                    var blockCoordinate = blockData->BlockCoordinate.ToVector3();
                    for (int j = 0; j < meshSize[0]; j++)
                    {
                        var index = meshStartIndex[0] + j;
                        resultVertices.AddNoResize(masterVertices[index] + blockCoordinate);
                        resultUVs.AddNoResize(masterUVs[index]);
                    }
                }
            }
        }
    }
}
