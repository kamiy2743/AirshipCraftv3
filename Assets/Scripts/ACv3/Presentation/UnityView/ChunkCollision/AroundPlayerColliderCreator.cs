using ACv3.Domain;
using Unity.Mathematics;

namespace ACv3.Presentation.ChunkCollision
{
    public class AroundPlayerColliderCreator
    {
        readonly CreatedColliders createdColliders;
        readonly ChunkColliderFactory chunkColliderFactory;
        readonly ChunkBoundsFactory chunkBoundsFactory;

        internal AroundPlayerColliderCreator(CreatedColliders createdColliders, ChunkColliderFactory chunkColliderFactory, ChunkBoundsFactory chunkBoundsFactory)
        {
            this.createdColliders = createdColliders;
            this.chunkColliderFactory = chunkColliderFactory;
            this.chunkBoundsFactory = chunkBoundsFactory;
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

                        if (createdColliders.Contains(cgc))
                        {
                            continue;
                        }

                        var chunkCollider = chunkColliderFactory.Create();
                        var chunkBounds = chunkBoundsFactory.Create(cgc);
                        chunkCollider.SetBounds(chunkBounds);

                        createdColliders.Add(cgc, chunkCollider);
                    }
                }
            }
        }
    }
}