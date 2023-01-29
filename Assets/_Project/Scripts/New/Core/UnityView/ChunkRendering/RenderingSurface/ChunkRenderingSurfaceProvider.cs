using Domain;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal class ChunkRenderingSurfaceProvider
    {
        private IChunkRenderingSurfaceRepository renderingSurfaceRepository;
        private ChunkRenderingSurfaceFactory renderingSurfaceFactory;

        internal ChunkRenderingSurfaceProvider(IChunkRenderingSurfaceRepository renderingSurfaceRepository, ChunkRenderingSurfaceFactory renderingSurfaceFactory)
        {
            this.renderingSurfaceRepository = renderingSurfaceRepository;
            this.renderingSurfaceFactory = renderingSurfaceFactory;
        }

        internal ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            try
            {
                return renderingSurfaceRepository.Fetch(chunkGridCoordinate);
            }
            catch
            {
                return renderingSurfaceFactory.Create(chunkGridCoordinate);
            }
        }
    }
}