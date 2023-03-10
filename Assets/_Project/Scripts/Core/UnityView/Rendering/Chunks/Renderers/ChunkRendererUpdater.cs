using Domain;
using UnityView.Rendering.Chunks;

namespace UnityView.Rendering.Chunks
{
    class ChunkRendererUpdater
    {
        readonly CreatedChunkRenderers _createdChunkRenderers;

        internal ChunkRendererUpdater(CreatedChunkRenderers createdChunkRenderers)
        {
            _createdChunkRenderers = createdChunkRenderers;
        }

        internal void Update(ChunkMesh chunkMesh)
        {
            if (!_createdChunkRenderers.TryGetValue(chunkMesh.ChunkGridCoordinate, out var chunkRenderer))
            {
                return;
            }

            chunkRenderer.SetMesh(chunkMesh);
        }
    }
}