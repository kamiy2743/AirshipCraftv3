using Domain;
using UnityView.Render.Chunks;

namespace UnityView.Render.Chunks
{
    internal class ChunkRendererUpdater
    {
        private CreatedChunkRenderers createdChunkRenderers;

        internal ChunkRendererUpdater(CreatedChunkRenderers createdChunkRenderers)
        {
            this.createdChunkRenderers = createdChunkRenderers;
        }

        internal void Update(ChunkMesh chunkMesh)
        {
            if (!createdChunkRenderers.TryGetValue(chunkMesh.chunkGridCoordinate, out var chunkRenderer))
            {
                return;
            }

            chunkRenderer.SetMesh(chunkMesh);
        }
    }
}