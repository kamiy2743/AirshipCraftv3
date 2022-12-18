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
using DataObject.Block;
using DataObject.Chunk;
using DataStore;
using MasterData.Block;

namespace ChunkConstruction
{
    /// <summary> 
    /// チャンク用のメッシュを作成する 
    /// </summary>
    public class ChunkMeshCreator : IDisposable
    {
        private ChunkMeshCreatorUtil _chunkMeshCreatorUtil;
        private MeshCombiner _meshCombiner;

        private List<ChunkMeshData> allocatedMeshDataList = new List<ChunkMeshData>();
        private Queue<ChunkMeshData> reusableMeshDataQueue = new Queue<ChunkMeshData>();

        private object syncObject = new Object();

        public ChunkMeshCreator(ChunkMeshCreatorUtil chunkMeshCreatorUtil, MeshCombiner meshCombiner)
        {
            _chunkMeshCreatorUtil = chunkMeshCreatorUtil;
            _meshCombiner = meshCombiner;
        }

        /// <summary>
        /// チャンク内のブロックメッシュを合成したメッシュを作成します
        /// キャンセルされた場合、頂点が0だった場合はnullを返す
        /// </summary>
        public ChunkMeshData CreateMeshData(ChunkData chunkData, CancellationToken ct)
        {
            ChunkMeshData meshData;
            try
            {
                // 描画面計算
                _chunkMeshCreatorUtil.CalcContactOtherBlockSurfaces(chunkData, ct);

                meshData = GetChunkMeshData();

                // メッシュ生成
                _meshCombiner.Combine(
                    chunkData.Blocks,
                    meshData.Vertices,
                    meshData.Triangles,
                    meshData.UVs,
                    ct);
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }

            return meshData;
        }

        /// <summary> 
        /// ChunkMeshDataをできるだけ再利用するようにして取得 
        /// </summary>
        private ChunkMeshData GetChunkMeshData()
        {
            lock (syncObject)
            {
                // 再利用キューから取得
                if (reusableMeshDataQueue.TryDequeue(out ChunkMeshData meshData))
                {
                    return meshData;
                }

                // 再利用できなければ新規作成
                meshData = new ChunkMeshData();
                allocatedMeshDataList.Add(meshData);

                // meshDataが解放されたら再利用キューに追加
                meshData.OnReleased.Subscribe(_ =>
                {
                    lock (syncObject)
                    {
                        reusableMeshDataQueue.Enqueue(meshData);
                    }
                });

                return meshData;
            }
        }

        public void Dispose()
        {
            foreach (var meshData in allocatedMeshDataList)
            {
                meshData.Dispose();
            }
        }
    }
}
