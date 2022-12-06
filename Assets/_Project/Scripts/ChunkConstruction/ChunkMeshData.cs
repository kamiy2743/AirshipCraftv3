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

namespace DataObject.Chunk
{
    public class ChunkMeshData : IDisposable
    {
        internal bool NoVertices => verticesList.Length == 0;
        private NativeList<Vector3> verticesList = new NativeList<Vector3>(Allocator.Persistent);
        private NativeList<int> trianglesList = new NativeList<int>(Allocator.Persistent);
        private NativeList<Vector2> uvsList = new NativeList<Vector2>(Allocator.Persistent);

        internal IObservable<Unit> OnReleased => _onReleasedSubject;
        private Subject<Unit> _onReleasedSubject = new Subject<Unit>();

        /// <typeparam name="int">BlockID</typeparam>
        /// <typeparam name="int2-0">v,tの開始位置</typeparam>
        /// <typeparam name="int2-1">v,tのサイズ</typeparam>
        // TODO BlockIDの個数を取得できるように
        private static readonly bool[] ContainsBlockIDArray = new bool[10];
        private static readonly NativeParallelHashMap<int, int2x2> MasterMeshDataInfoHashMap = new NativeParallelHashMap<int, int2x2>(10, Allocator.Persistent);
        private static readonly NativeList<Vector3> MasterVertices = new NativeList<Vector3>(Allocator.Persistent);
        private static readonly NativeList<int> MasterTriangles = new NativeList<int>(Allocator.Persistent);
        private static readonly NativeList<Vector2> MasterUVs = new NativeList<Vector2>(Allocator.Persistent);
        private static readonly object SyncObject = new object();

        internal ChunkMeshData() { }
        internal unsafe void Init(ChunkData chunkData, CancellationToken ct)
        {
            lock (SyncObject)
            {
                verticesList.Clear();
                trianglesList.Clear();
                uvsList.Clear();

                MasterMeshDataInfoHashMap.Clear();
                MasterVertices.Clear();
                MasterTriangles.Clear();
                MasterUVs.Clear();

                fixed (bool* containsBlockIDArrayFirst = &ContainsBlockIDArray[0])
                fixed (BlockData* blocksFirst = &chunkData.Blocks[0])
                {
                    var job = new SetUpContainsBlockIDArrayJob
                    {
                        containsBlockIDArrayFirst = containsBlockIDArrayFirst,
                        blocksFirst = blocksFirst,
                        containsBlockIDArrayLength = ContainsBlockIDArray.Length
                    };

                    job.Schedule().Complete();
                }

                for (int i = 0; i < ContainsBlockIDArray.Length; i++)
                {
                    if (!ContainsBlockIDArray[i]) continue;

                    var blockID = i;
                    var masterMeshData = MasterBlockDataStore.GetData((BlockID)blockID).MeshData;

                    var masterMeshDataInfo = new int2x2(
                        MasterVertices.Length, masterMeshData.Vertices.Length,
                        MasterTriangles.Length, masterMeshData.Triangles.Length
                    );
                    MasterMeshDataInfoHashMap.Add((int)blockID, masterMeshDataInfo);

                    unsafe
                    {
                        fixed (void*
                            vp = &masterMeshData.Vertices[0],
                            tp = &masterMeshData.Triangles[0],
                            up = &masterMeshData.UVs[0])
                        {
                            MasterVertices.AddRange(vp, masterMeshData.Vertices.Length);
                            MasterTriangles.AddRange(tp, masterMeshData.Triangles.Length);
                            MasterUVs.AddRange(up, masterMeshData.Vertices.Length);
                        }
                    }
                }

                if (MasterVertices.Length == 0) return;
                ct.ThrowIfCancellationRequested();

                fixed (BlockData* blocksFirst = &chunkData.Blocks[0])
                {
                    var job = new CombineMeshJob
                    {
                        blocksFirst = blocksFirst,
                        masterMeshDataInfoHashMap = MasterMeshDataInfoHashMap,
                        masterVertices = MasterVertices,
                        masterTriangles = MasterTriangles,
                        masterUVs = MasterUVs,
                        resultVertices = verticesList,
                        resultTriangles = trianglesList,
                        resultUVs = uvsList
                    };

                    job.Schedule().Complete();
                }
            }
        }

        public void Release()
        {
            _onReleasedSubject.OnNext(default);
        }

        public void Dispose()
        {
            verticesList.Dispose();
            trianglesList.Dispose();
            uvsList.Dispose();
        }

        public static void DisposeStaticResource()
        {
            MasterMeshDataInfoHashMap.Dispose();
            MasterVertices.Dispose();
            MasterTriangles.Dispose();
            MasterUVs.Dispose();
        }

        public static implicit operator NativeMeshData(ChunkMeshData meshData)
        {
            if (meshData is null) return null;

            return new NativeMeshData(
                meshData.verticesList.AsArray(),
                meshData.trianglesList.AsArray(),
                meshData.uvsList.AsArray()
            );
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
