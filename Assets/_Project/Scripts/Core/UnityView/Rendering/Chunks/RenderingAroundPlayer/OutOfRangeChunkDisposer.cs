using Domain;
using Unity.Mathematics;

namespace UnityView.Rendering.Chunks
{
    internal class OutOfRangeChunkDisposer
    {
        private CreatedChunkRenderers createdChunkRenderers;

        internal OutOfRangeChunkDisposer(CreatedChunkRenderers createdChunkRenderers)
        {
            this.createdChunkRenderers = createdChunkRenderers;
        }

        internal void Execute(ChunkGridCoordinate playerChunk, int maxRenderingRadius)
        {
            foreach (var cgc in createdChunkRenderers.CreatedCoordinatesDeepCopy)
            {
                if (math.distance(cgc.x, playerChunk.x) > maxRenderingRadius ||
                    math.distance(cgc.y, playerChunk.y) > maxRenderingRadius ||
                    math.distance(cgc.z, playerChunk.z) > maxRenderingRadius)
                {
                    createdChunkRenderers.Dispose(cgc);
                }
            }
        }
    }
}