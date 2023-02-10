using System;
using System.Linq;
using System.Collections.Generic;
using Domain;

namespace UnityView.Rendering.Chunks
{
    class CreatedChunkRenderers : IDisposable
    {
        readonly Dictionary<ChunkGridCoordinate, ChunkRenderer> _renderers = new Dictionary<ChunkGridCoordinate, ChunkRenderer>();
        internal List<ChunkGridCoordinate> CreatedCoordinatesDeepCopy => _renderers.Keys.ToList();

        internal void Add(ChunkGridCoordinate chunkGridCoordinate, ChunkRenderer chunkRenderer)
        {
            _renderers.Add(chunkGridCoordinate, chunkRenderer);
        }

        internal bool Contains(ChunkGridCoordinate chunkGridCoordinate)
        {
            return _renderers.ContainsKey(chunkGridCoordinate);
        }

        internal bool TryGetValue(ChunkGridCoordinate chunkGridCoordinate, out ChunkRenderer result)
        {
            return _renderers.TryGetValue(chunkGridCoordinate, out result);
        }

        internal void Dispose(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (!TryGetValue(chunkGridCoordinate, out var result))
            {
                return;
            }
            result.Dispose();
            _renderers.Remove(chunkGridCoordinate);
        }

        public void Dispose()
        {
            foreach (var renderer in _renderers.Values)
            {
                renderer.Dispose();
            }
        }
    }
}