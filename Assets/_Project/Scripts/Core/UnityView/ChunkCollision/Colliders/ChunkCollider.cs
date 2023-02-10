using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityView.ChunkCollision
{
    class ChunkCollider : MonoBehaviour, IDisposable
    {
        readonly List<Collider> _colliders = new List<Collider>();

        internal void SetBounds(ChunkBounds chunkBounds)
        {
            transform.position = chunkBounds.ChunkGridCoordinate.ToPivotCoordinate();

            foreach (var collider in _colliders)
            {
                Destroy(collider);
            }

            foreach (var blockBounds in chunkBounds.BlockBoundsCollection)
            {
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.center = blockBounds.Center;
                collider.size = blockBounds.Size;

                _colliders.Add(collider);
            }
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}