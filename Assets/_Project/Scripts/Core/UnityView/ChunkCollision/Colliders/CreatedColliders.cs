using System.Linq;
using System.Collections.Generic;
using Domain;

namespace UnityView.ChunkCollision
{
    class CreatedColliders
    {
        readonly Dictionary<ChunkGridCoordinate, ChunkCollider> _colliders = new Dictionary<ChunkGridCoordinate, ChunkCollider>();
        internal List<ChunkGridCoordinate> CreatedCoordinatesDeepCopy => _colliders.Keys.ToList();

        internal void Add(ChunkGridCoordinate chunkGridCoordinate, ChunkCollider collider)
        {
            _colliders.Add(chunkGridCoordinate, collider);
        }

        internal bool Contains(ChunkGridCoordinate chunkGridCoordinate)
        {
            return _colliders.ContainsKey(chunkGridCoordinate);
        }

        internal bool TryGetValue(ChunkGridCoordinate chunkGridCoordinate, out ChunkCollider result)
        {
            return _colliders.TryGetValue(chunkGridCoordinate, out result);
        }

        internal void Dispose(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (!TryGetValue(chunkGridCoordinate, out var result))
            {
                UnityEngine.Debug.Log(chunkGridCoordinate);
                return;
            }
            result.Dispose();
            _colliders.Remove(chunkGridCoordinate);
        }
    }
}