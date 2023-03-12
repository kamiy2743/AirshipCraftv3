using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ACv3.Domain;
using ACv3.Domain.Chunks;
using Unity.Mathematics;
using UnityEngine;
using ACv3.UnityView.Players;

namespace ACv3.UnityView.Rendering.Chunks
{
    public class InSightChunkCreator
    {
        readonly ChunkRendererFactory chunkRendererFactory;
        readonly ChunkMeshFactory chunkMeshFactory;
        readonly CreatedChunkRenderers createdChunkRenderers;
        readonly PlayerCamera playerCamera;

        internal InSightChunkCreator(ChunkRendererFactory chunkRendererFactory, ChunkMeshFactory chunkMeshFactory, CreatedChunkRenderers createdChunkRenderers, PlayerCamera playerCamera)
        {
            this.chunkRendererFactory = chunkRendererFactory;
            this.chunkMeshFactory = chunkMeshFactory;
            this.createdChunkRenderers = createdChunkRenderers;
            this.playerCamera = playerCamera;
        }

        internal async UniTask ExecuteAsync(ChunkGridCoordinate playerChunk, int maxRenderingRadius, CancellationToken ct)
        {
            var createdMeshes = new Queue<(ChunkGridCoordinate, ChunkMesh)>();
            CreateMeshesTask(playerChunk, maxRenderingRadius, createdMeshes, ct).Forget();

            while (true)
            {
                await UniTask.WaitForEndOfFrame();

                if (ct.IsCancellationRequested)
                {
                    return;
                }

                while (createdMeshes.TryDequeue(out var item))
                {
                    var cgc = item.Item1;
                    var mesh = item.Item2;

                    var chunkRenderer = chunkRendererFactory.Create();
                    chunkRenderer.SetMesh(mesh);
                    createdChunkRenderers.Add(cgc, chunkRenderer);
                }
            }
        }

        async UniTask CreateMeshesTask(
            ChunkGridCoordinate playerChunk,
            int maxRenderingRadius,
            Queue<(ChunkGridCoordinate, ChunkMesh)> createdMeshes,
            CancellationToken ct)
        {
            var inSightChecker = new InSightChecker(playerCamera.ViewportMatrix);
            var createChunkQueue = new CreateChunkQueue((int)math.pow(maxRenderingRadius * 2 + 1, 3));

            await UniTask.SwitchToThreadPool();

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

                        if (!inSightChecker.Check(CalcBounds(cgc)))
                        {
                            continue;
                        }

                        var distance = (x * x) + (y * y) + (z * z);
                        createChunkQueue.Enqueue(distance, cgc);
                    }
                }
            }

            while (createChunkQueue.TryDequeue(out var cgc))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var mesh = chunkMeshFactory.Create(cgc);
                createdMeshes.Enqueue((cgc, mesh));
            }

            await UniTask.SwitchToMainThread();
        }

        Bounds CalcBounds(ChunkGridCoordinate cgc)
        {
            var size = Vector3.one * Chunk.BlockSide;
            var center = (Vector3)cgc.ToPivotCoordinate() + (size * 0.5f);
            return new Bounds(center, size);
        }
    }
}