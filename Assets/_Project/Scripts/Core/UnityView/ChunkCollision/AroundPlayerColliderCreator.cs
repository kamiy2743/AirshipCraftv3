using Domain;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    class AroundPlayerColliderCreator
    {
        readonly CreatedColliders _createdColliders;
        readonly ChunkColliderFactory _chunkColliderFactory;
        readonly ChunkBoundsFactory _chunkBoundsFactory;

        internal AroundPlayerColliderCreator(CreatedColliders createdColliders, ChunkColliderFactory chunkColliderFactory, ChunkBoundsFactory chunkBoundsFactory)
        {
            _createdColliders = createdColliders;
            _chunkColliderFactory = chunkColliderFactory;
            _chunkBoundsFactory = chunkBoundsFactory;
        }

        internal void Execute(ChunkGridCoordinate playerChunk, int radius)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        if (!playerChunk.TryAdd(new int3(x, y, z), out var cgc))
                        {
                            continue;
                        }

                        if (_createdColliders.Contains(cgc))
                        {
                            continue;
                        }

                        var chunkCollider = _chunkColliderFactory.Create();
                        var chunkBounds = _chunkBoundsFactory.Create(cgc);
                        chunkCollider.SetBounds(chunkBounds);

                        _createdColliders.Add(cgc, chunkCollider);
                    }
                }
            }
        }
    }
}