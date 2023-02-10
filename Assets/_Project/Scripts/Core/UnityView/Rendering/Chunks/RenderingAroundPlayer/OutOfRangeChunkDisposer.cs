using Domain;
using Unity.Mathematics;

namespace UnityView.Rendering.Chunks
{
    class OutOfRangeChunkDisposer
    {
        readonly CreatedChunkRenderers _createdChunkRenderers;

        internal OutOfRangeChunkDisposer(CreatedChunkRenderers createdChunkRenderers)
        {
            _createdChunkRenderers = createdChunkRenderers;
        }

        internal void Execute(ChunkGridCoordinate playerChunk, int maxRenderingRadius)
        {
            foreach (var cgc in _createdChunkRenderers.CreatedCoordinatesDeepCopy)
            {
                if (math.distance(cgc.X, playerChunk.X) > maxRenderingRadius ||
                    math.distance(cgc.Y, playerChunk.Y) > maxRenderingRadius ||
                    math.distance(cgc.Z, playerChunk.Z) > maxRenderingRadius)
                {
                    _createdChunkRenderers.Dispose(cgc);
                }
            }
        }
    }
}