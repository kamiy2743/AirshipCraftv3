using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using Domain.Chunks;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;
using UnityView.Players;

namespace UnityView.Rendering.Chunks
{
    class InSightChunkCreator
    {
        readonly ChunkRendererFactory _chunkRendererFactory;
        readonly ChunkMeshFactory _chunkMeshFactory;
        readonly CreatedChunkRenderers _createdChunkRenderers;
        readonly PlayerCamera _playerCamera;

        internal InSightChunkCreator(ChunkRendererFactory chunkRendererFactory, ChunkMeshFactory chunkMeshFactory, CreatedChunkRenderers createdChunkRenderers, PlayerCamera playerCamera)
        {
            _chunkRendererFactory = chunkRendererFactory;
            _chunkMeshFactory = chunkMeshFactory;
            _createdChunkRenderers = createdChunkRenderers;
            _playerCamera = playerCamera;
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

                    var chunkRenderer = _chunkRendererFactory.Create();
                    chunkRenderer.SetMesh(mesh);
                    _createdChunkRenderers.Add(cgc, chunkRenderer);
                }
            }
        }

        async UniTask CreateMeshesTask(
            ChunkGridCoordinate playerChunk,
            int maxRenderingRadius,
            Queue<(ChunkGridCoordinate, ChunkMesh)> createdMeshes,
            CancellationToken ct)
        {
            var inSightChecker = new InSightChecker(_playerCamera.ViewportMatrix);
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

                        if (_createdChunkRenderers.Contains(cgc))
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

                var mesh = _chunkMeshFactory.Create(cgc);
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