using UnityEngine;
using Domain.Chunks;
using UnityView.ChunkRendering.Mesh;
using Unity.Mathematics;

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

        internal void Execute(ChunkGridCoordinate playerChunk, int maxRenderingRadius)
        {
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

                        if (!inSightChecker.Check(CalcBounds(cgc)))
                        {
                            continue;
                        }

                        if (createdChunkRenderers.Contains(cgc))
                        {
                            continue;
                        }

                        var mesh = chunkMeshDataFactory.Create(cgc);
                        var chunkRenderer = chunkRendererFactory.Create(cgc);
                        chunkRenderer.SetMesh(mesh);

                        createdChunkRenderers.Add(cgc, chunkRenderer);
                    }
                }
            }
        }

        private Bounds CalcBounds(ChunkGridCoordinate cgc)
        {
            var center = (new Vector3(cgc.x, cgc.y, cgc.z) * Chunk.BlockSide) + (Vector3.one * Chunk.BlockSide * 0.5f);
            return new Bounds(center, Vector3.one * Chunk.BlockSide);
        }
    }
}