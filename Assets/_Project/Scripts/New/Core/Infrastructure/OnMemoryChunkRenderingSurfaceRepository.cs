using System.Collections.Generic;
using Domain;
using UnityView.ChunkRender.RenderingSurface;

namespace Infrastructure
{
    internal class OnMemoryChunkRenderingSurfaceRepository : IChunkRenderingSurfaceRepository
    {
        internal Dictionary<ChunkGridCoordinate, ChunkRenderingSurface> surfaces = new Dictionary<ChunkGridCoordinate, ChunkRenderingSurface>();

        public void Store(ChunkRenderingSurface renderingSurface)
        {
            surfaces[renderingSurface.chunkGridCoordinate] = renderingSurface.DeepCopy();
        }

        public ChunkRenderingSurface Fetch(ChunkGridCoordinate chunkGridCoordinate)
        {
            return surfaces[chunkGridCoordinate].DeepCopy();
        }
    }
}