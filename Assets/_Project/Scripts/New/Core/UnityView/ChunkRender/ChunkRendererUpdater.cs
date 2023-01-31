using Domain;
using UnityView.ChunkRender.Mesh;

namespace UnityView.ChunkRender
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