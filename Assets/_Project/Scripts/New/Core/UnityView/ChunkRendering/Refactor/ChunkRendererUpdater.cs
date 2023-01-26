using UnityEngine;
using Domain.Chunks;
using UnityView.ChunkRendering.Model.ChunkMesh;

namespace UnityView.ChunkRendering
{
    internal class ChunkRendererUpdater
    {
        private CreatedChunkRenderers createdChunkRenderers;

        internal ChunkRendererUpdater(CreatedChunkRenderers createdChunkRenderers)
        {
            this.createdChunkRenderers = createdChunkRenderers;
        }

        internal void Update(ChunkGridCoordinate updateCoordinate, ChunkMesh mesh)
        {
            if (!createdChunkRenderers.TryGetValue(updateCoordinate, out var chunkRenderer))
            {
                return;
            }

            chunkRenderer.SetMesh(mesh);
        }
    }
}