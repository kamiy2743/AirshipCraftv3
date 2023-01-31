using Domain;
using Unity.Mathematics;

namespace UnityView.ChunkCollision
{
    internal class AroundPlayerColliderCreator
    {
        private CreatedColliders createdColliders;
        private ChunkColliderFactory chunkColliderFactory;

        internal AroundPlayerColliderCreator(CreatedColliders createdColliders, ChunkColliderFactory chunkColliderFactory)
        {
            this.createdColliders = createdColliders;
            this.chunkColliderFactory = chunkColliderFactory;
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
                        createdColliders.Add(cgc, chunkCollider);
                    }
                }
            }
        }
    }
}