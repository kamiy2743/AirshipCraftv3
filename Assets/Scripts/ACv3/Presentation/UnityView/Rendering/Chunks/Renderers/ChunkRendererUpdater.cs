namespace ACv3.Presentation.Rendering.Chunks
{
    public class ChunkRendererUpdater
    {
        readonly CreatedChunkRenderers createdChunkRenderers;

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