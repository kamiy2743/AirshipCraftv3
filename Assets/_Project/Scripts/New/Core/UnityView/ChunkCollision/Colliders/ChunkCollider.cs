using System;
using UnityEngine;

namespace UnityView.ChunkCollision
{
    internal class ChunkCollider : MonoBehaviour, IDisposable
    {
        internal void SetBounds(ChunkBounds chunkBounds)
        {
            transform.position = chunkBounds.pivotCoordinate;

            foreach (var blockBounds in chunkBounds.BlockBoundsCollection)
            {
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.center = blockBounds.center;
                collider.size = blockBounds.size;
            }
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}