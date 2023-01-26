using System.Collections.Generic;
using Domain.Chunks;

namespace UnityView.ChunkRendering
{
    internal class CreatedChunkRenderers
    {
        private Dictionary<ChunkGridCoordinate, ChunkRenderer> renderers = new Dictionary<ChunkGridCoordinate, ChunkRenderer>();

        internal void Add(ChunkGridCoordinate chunkGridCoordinate, ChunkRenderer chunkRenderer)
        {
            renderers.Add(chunkGridCoordinate, chunkRenderer);
        }

        internal bool Contains(ChunkGridCoordinate chunkGridCoordinate)
        {
            return renderers.ContainsKey(chunkGridCoordinate);
        }

        internal bool TryGetValue(ChunkGridCoordinate chunkGridCoordinate, out ChunkRenderer result)
        {
            return renderers.TryGetValue(chunkGridCoordinate, out result);
        }
    }
}