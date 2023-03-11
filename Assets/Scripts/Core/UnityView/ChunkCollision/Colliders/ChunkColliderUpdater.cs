namespace UnityView.ChunkCollision
{
    public class ChunkColliderUpdater
    {
        readonly CreatedColliders createdColliders;

        internal ChunkColliderUpdater(CreatedColliders createdColliders)
        {
            this.createdColliders = createdColliders;
        }

        internal void Update(ChunkBounds chunkBounds)
        {
            if (!createdColliders.TryGetValue(chunkBounds.chunkGridCoordinate, out var chunkCollider))
            {
                return;
            }

            chunkCollider.SetBounds(chunkBounds);
        }
    }
}