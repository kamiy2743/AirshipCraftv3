using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityView.ChunkCollision
{
    internal class ChunkCollider : MonoBehaviour, IDisposable
    {
        private List<Collider> colliders = new List<Collider>();

        internal void SetBounds(ChunkBounds chunkBounds)
        {
            transform.position = chunkBounds.chunkGridCoordinate.ToPivotCoordinate();

            foreach (var collider in colliders)
            {
                Destroy(collider);
            }

            foreach (var blockBounds in chunkBounds.BlockBoundsCollection)
            {
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.center = blockBounds.center;
                collider.size = blockBounds.size;

                colliders.Add(collider);
            }
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}