using System.Collections.Generic;
using System.Linq;
using ACv3.Domain;

namespace ACv3.UnityView.ChunkCollision
{
    public class CreatedColliders
    {
        readonly Dictionary<ChunkGridCoordinate, ChunkCollider> colliders = new();
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