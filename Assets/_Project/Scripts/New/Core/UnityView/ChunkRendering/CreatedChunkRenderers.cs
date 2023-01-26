using Domain.Chunks;

namespace UnityView.ChunkRendering
{
    internal class CreatedChunkRenderers
    {
        internal bool TryGetValue(ChunkGridCoordinate key, out ChunkRenderer result)
        {
            result = null;
            return false;
        }
    }
}