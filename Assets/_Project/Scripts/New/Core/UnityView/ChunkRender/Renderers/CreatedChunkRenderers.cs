using System;
using System.Linq;
using System.Collections.Generic;
using Domain;

namespace UnityView.ChunkRender
{
    internal class CreatedChunkRenderers : IDisposable
    {
        private Dictionary<ChunkGridCoordinate, ChunkRenderer> renderers = new Dictionary<ChunkGridCoordinate, ChunkRenderer>();
        internal List<ChunkGridCoordinate> CreatedCoordinatesDeepCopy => renderers.Keys.ToList();

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

        internal void Dispose(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (!TryGetValue(chunkGridCoordinate, out var result))
            {
                return;
            }
            result.Dispose();
            renderers.Remove(chunkGridCoordinate);
        }

        public void Dispose()
        {
            foreach (var renderer in renderers.Values)
            {
                renderer.Dispose();
            }
        }
    }
}