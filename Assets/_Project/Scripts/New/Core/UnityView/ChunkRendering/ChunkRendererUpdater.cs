using Domain.Chunks;
using UnityView.ChunkRendering.Mesh;

namespace UnityView.ChunkRendering
{
    internal class ChunkRendererUpdater
    {
        private CreatedChunkRenderers createdChunkRenderers;

        internal ChunkRendererUpdater(CreatedChunkRenderers createdChunkRenderers)
        {
            this.createdChunkRenderers = createdChunkRenderers;
        }

        internal void Update(ChunkGridCoordinate updateCoordinate, ChunkMeshData mesh)
        {
            if (!createdChunkRenderers.TryGetValue(updateCoordinate, out var chunkRenderer))
            {
                return;
            }

            chunkRenderer.SetMesh(mesh);
        }
    }
}