using Domain.Chunks;

namespace RenderingOptimization.RenderingSurface
{
    public interface IChunkRenderingSurfaceRepository
    {
        void Store(ChunkRenderingSurface renderingSurface);

        /// <exception cref="KeyNotFoundException"></exception>
        ChunkRenderingSurface Fetch(ChunkGridCoordinate chunkGridCoordinate);
    }
}