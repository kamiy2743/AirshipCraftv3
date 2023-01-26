using Domain;

namespace UnityView.ChunkRendering.RenderingSurface
{
    internal class ChunkRenderingSurfaceProvider : IChunkRenderingSurfaceProvider
    {
        private IChunkRenderingSurfaceRepository renderingSurfaceRepository;
        private IChunkRenderingSurfaceFactory renderingSurfaceFactory;

        internal ChunkRenderingSurfaceProvider(IChunkRenderingSurfaceRepository renderingSurfaceRepository, IChunkRenderingSurfaceFactory renderingSurfaceFactory)
        {
            this.renderingSurfaceRepository = renderingSurfaceRepository;
            this.renderingSurfaceFactory = renderingSurfaceFactory;
        }

        public ChunkRenderingSurface GetRenderingSurface(ChunkGridCoordinate chunkGridCoordinate)
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