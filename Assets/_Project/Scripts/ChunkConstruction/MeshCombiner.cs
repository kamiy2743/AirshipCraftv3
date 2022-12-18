using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Util;
using UniRx;
using System;
using System.Threading;
using MasterData.Block;
using DataObject.Block;
using DataObject.Chunk;

namespace ChunkConstruction
{
    public class MeshCombiner : IDisposable
    {
        private MasterBlockDataStore _masterBlockDataStore;

        /// <typeparam name="int">BlockID</typeparam>
        /// <typeparam name="int2-0">v,tの開始位置</typeparam>
        /// <typeparam name="int2-1">v,tのサイズ</typeparam>
        // TODO BlockIDの個数を取得できるように
        private bool[] containsBlockIDArray = new bool[10];
        private NativeParallelHashMap<int, int2x2> masterMeshDataInfoHashMap = new NativeParallelHashMap<int, int2x2>(10, Allocator.Persistent);
        private NativeList<Vector3> masterVertices = new NativeList<Vector3>(Allocator.Persistent);
        private NativeList<int> masterTriangles = new NativeList<int>(Allocator.Persistent);
        private NativeList<Vector2> masterUVs = new NativeList<Vector2>(Allocator.Persistent);

        private object syncObject = new object();

        public MeshCombiner(MasterBlockDataStore masterBlockDataStore)
        {
            _masterBlockDataStore = masterBlockDataStore;
        }

        public void Dispose()
        {
            masterMeshDataInfoHashMap.Dispose();
            masterVertices.Dispose();
            masterTriangles.Dispose();
            masterUVs.Dispose();
        }

        internal unsafe void Combine(
            BlockData[] blocks,
            NativeList<Vector3> resultVertices,
            NativeList<int> resultTriangles,
            NativeList<Vector2> resultUVs,
            CancellationToken ct)
        {
            lock (syncObject)
            {
                if (resultVertices.Length > 0)
                {
                    resultVertices.Clear();
                    resultTriangles.Clear();
                    resultUVs.Clear();
                }

                masterMeshDataInfoHashMap.Clear();
                masterVertices.Clear();
                masterTriangles.Clear();
                masterUVs.Clear();

                fixed (bool* containsBlockIDArrayFirst = &containsBlockIDArray[0])
                fixed (BlockData* blocksFirst = &blocks[0])
                {
                    var job = new SetUpContainsBlockIDArrayJob
                    {
                        containsBlockIDArrayFirst = containsBlockIDArrayFirst,
                        blocksFirst = blocksFirst,
                        containsBlockIDArrayLength = containsBlockIDArray.Length
                    };

                    job.Schedule().Complete();
                }

                for (int i = 0; i < containsBlockIDArray.Length; i++)
                {
                    if (!containsBlockIDArray[i]) continue;

                    var blockID = i;
                    var masterMeshData = _masterBlockDataStore.GetData((BlockID)blockID).MeshData;

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

                if (masterVertices.Length == 0) return;
                ct.ThrowIfCancellationRequested();

                fixed (BlockData* blocksFirst = &blocks[0])
                {
                    var job = new CombineMeshJob
                    {
                        blocksFirst = blocksFirst,
                        masterMeshDataInfoHashMap = masterMeshDataInfoHashMap,
                        masterVertices = masterVertices,
                        masterTriangles = masterTriangles,
                        masterUVs = masterUVs,
                        resultVertices = resultVertices,
                        resultTriangles = resultTriangles,
                        resultUVs = resultUVs
                    };

                    job.Schedule().Complete();
                }
            }
        }

        [BurstCompile]
        private unsafe struct SetUpContainsBlockIDArrayJob : IJob
        {
            [NativeDisableUnsafePtrRestriction][ReadOnly] public bool* containsBlockIDArrayFirst;
            [NativeDisableUnsafePtrRestriction][ReadOnly] public BlockData* blocksFirst;
            [ReadOnly] public int containsBlockIDArrayLength;

            public void Execute()
            {
                // 初期化
                for (int i = 0; i < containsBlockIDArrayLength; i++)
                {
                    *(containsBlockIDArrayFirst + i) = false;
                }

                for (int i = 0; i < ChunkData.BlockCountInChunk; i++)
                {
                    var block = blocksFirst + i;
                    if (block->IsRenderSkip) continue;
                    *(containsBlockIDArrayFirst + (int)block->ID) = true;
                }
            }
        }

        [BurstCompile]
        private unsafe struct CombineMeshJob : IJob
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
                            resultTriangles.Add(t + vc);
                        }
                    }

                    var blockCoordinate = blockData->BlockCoordinate.ToVector3();
                    for (int j = 0; j < meshSize[0]; j++)
                    {
                        var index = meshStartIndex[0] + j;
                        resultVertices.Add(masterVertices[index] + blockCoordinate);
                        resultUVs.Add(masterUVs[index]);
                    }
                }
            }
        }
    }
}
