using Domain;
using UnityView.ChunkRender;

namespace UnityView.ChunkRender
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