namespace UnityView.ChunkCollision
{
    class ChunkColliderUpdater
    {
        readonly CreatedColliders _createdColliders;

        internal ChunkColliderUpdater(CreatedColliders createdColliders)
        {
            _createdColliders = createdColliders;
        }

        internal void Update(ChunkBounds chunkBounds)
        {
            if (!_createdColliders.TryGetValue(chunkBounds.ChunkGridCoordinate, out var chunkCollider))
            {
                return;
            }

            chunkCollider.SetBounds(chunkBounds);
        }
    }
}