using Domain;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    class OutOfRangeColliderDisposer
    {
        readonly CreatedColliders _createdColliders;

        internal OutOfRangeColliderDisposer(CreatedColliders createdColliders)
        {
            _createdColliders = createdColliders;
        }

        internal void Execute(ChunkGridCoordinate playerChunk, int radius)
        {
            foreach (var cgc in _createdColliders.CreatedCoordinatesDeepCopy)
            {
                if (math.distance(cgc.X, playerChunk.X) > radius ||
                    math.distance(cgc.Y, playerChunk.Y) > radius ||
                    math.distance(cgc.Z, playerChunk.Z) > radius)
                {
                    _createdColliders.Dispose(cgc);
                }
            }
        }
    }
}