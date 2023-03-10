namespace UnityView.ChunkCollision
{
    internal class ChunkColliderUpdater
    {
        private CreatedColliders createdColliders;

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