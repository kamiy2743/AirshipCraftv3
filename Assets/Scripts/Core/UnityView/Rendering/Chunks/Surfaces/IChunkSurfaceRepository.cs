using System.Collections.Generic;
using ACv3.Domain;

namespace ACv3.UnityView.Rendering.Chunks
{
    public interface IChunkSurfaceRepository
    {
        void Store(ChunkSurface chunkSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}