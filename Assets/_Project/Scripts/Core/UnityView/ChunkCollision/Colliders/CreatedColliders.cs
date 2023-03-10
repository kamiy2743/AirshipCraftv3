using System.Linq;
using System.Collections.Generic;
using Domain;

namespace UnityView.ChunkCollision
{
    internal class CreatedColliders
    {
        private Dictionary<ChunkGridCoordinate, ChunkCollider> colliders = new Dictionary<ChunkGridCoordinate, ChunkCollider>();
        internal List<ChunkGridCoordinate> CreatedCoordinatesDeepCopy => colliders.Keys.ToList();

        internal void Add(ChunkGridCoordinate chunkGridCoordinate, ChunkCollider collider)
        {
            colliders.Add(chunkGridCoordinate, collider);
        }

        internal bool Contains(ChunkGridCoordinate chunkGridCoordinate)
        {
            return colliders.ContainsKey(chunkGridCoordinate);
        }

        internal bool TryGetValue(ChunkGridCoordinate chunkGridCoordinate, out ChunkCollider result)
        {
            return colliders.TryGetValue(chunkGridCoordinate, out result);
        }

        internal void Dispose(ChunkGridCoordinate chunkGridCoordinate)
        {
            if (!TryGetValue(chunkGridCoordinate, out var result))
            {
                UnityEngine.Debug.Log(chunkGridCoordinate);
                return;
            }
            result.Dispose();
            colliders.Remove(chunkGridCoordinate);
        }
    }
}