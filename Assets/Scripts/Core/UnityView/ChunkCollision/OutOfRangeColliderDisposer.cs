using Domain;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    public class OutOfRangeColliderDisposer
    {
        readonly CreatedColliders createdColliders;

        internal OutOfRangeColliderDisposer(CreatedColliders createdColliders)
        {
            this.createdColliders = createdColliders;
        }

        internal void Execute(ChunkGridCoordinate playerChunk, int radius)
        {
            foreach (var cgc in createdColliders.CreatedCoordinatesDeepCopy)
            {
                if (math.distance(cgc.x, playerChunk.x) > radius ||
                    math.distance(cgc.y, playerChunk.y) > radius ||
                    math.distance(cgc.z, playerChunk.z) > radius)
                {
                    createdColliders.Dispose(cgc);
                }
            }
        }
    }
}