using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using Domain.Chunks;
using UnityView.ChunkRendering.Mesh;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;

namespace UnityView.ChunkRendering
{
    internal class InSightChunkCreator
    {
        private ChunkRendererFactory chunkRendererFactory;
        private ChunkMeshDataFactory chunkMeshDataFactory;
        private CreatedChunkRenderers createdChunkRenderers;
        private InSightChecker inSightChecker;

        internal InSightChunkCreator(ChunkRendererFactory chunkRendererFactory, ChunkMeshDataFactory chunkMeshDataFactory, CreatedChunkRenderers createdChunkRenderers, InSightChecker inSightChecker)
        {
            this.chunkRendererFactory = chunkRendererFactory;
            this.chunkMeshDataFactory = chunkMeshDataFactory;
            this.createdChunkRenderers = createdChunkRenderers;
            this.inSightChecker = inSightChecker;
        }

        internal async UniTask ExecuteAsync(ChunkGridCoordinate playerChunk, int maxRenderingRadius, CancellationToken ct)
        {
            var createdMeshes = new Queue<(ChunkGridCoordinate, ChunkMeshData)>();
            CreateMeshesTask(playerChunk, maxRenderingRadius, createdMeshes, ct).Forget();

            while (true)
            {
                await UniTask.WaitForEndOfFrame();

                if (ct.IsCancellationRequested)
                {
                    return;
                }

                if (!createdMeshes.TryDequeue(out var item))
                {
                    continue;
                }

                var cgc = item.Item1;
                var mesh = item.Item2;

                var chunkRenderer = chunkRendererFactory.Create();
                chunkRenderer.SetMesh(mesh);
                createdChunkRenderers.Add(cgc, chunkRenderer);
            }
        }

        private async UniTask CreateMeshesTask(
            ChunkGridCoordinate playerChunk,
            int maxRenderingRadius,
            Queue<(ChunkGridCoordinate, ChunkMeshData)> createdMeshes,
            CancellationToken ct)
        {
            var createChunkQueue = new CreateChunkQueue((int)math.pow(maxRenderingRadius * 2 + 1, 3));

            for (int x = -maxRenderingRadius; x <= maxRenderingRadius; x++)
            {
                for (int y = -maxRenderingRadius; y <= maxRenderingRadius; y++)
                {
                    for (int z = -maxRenderingRadius; z <= maxRenderingRadius; z++)
                    {
                        if (!playerChunk.TryAdd(new int3(x, y, z), out var cgc))
                        {
                            continue;
                        }

                        if (createdChunkRenderers.Contains(cgc))
                        {
                            continue;
                        }

                        // TODO Checkをマルチスレッド対応
                        if (!inSightChecker.Check(CalcBounds(cgc)))
                        {
                            continue;
                        }

                        var distance = (x * x) + (y * y) + (z * z);
                        createChunkQueue.Enqueue(distance, cgc);
                    }
                }
            }

            // TODO このメッソド全体をマルチスレッド化する
            await UniTask.SwitchToThreadPool();

            while (createChunkQueue.TryDequeue(out var cgc))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var mesh = chunkMeshDataFactory.Create(cgc);
                createdMeshes.Enqueue((cgc, mesh));
            }

            await UniTask.SwitchToMainThread();
        }

        private Bounds CalcBounds(ChunkGridCoordinate cgc)
        {
            var size = Vector3.one * Chunk.BlockSide;
            var center = (Vector3)cgc.ToPivotCoordinate() + (size * 0.5f);
            return new Bounds(center, size);
        }
    }
}