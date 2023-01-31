using Domain;

namespace UnityView.ChunkRender
{
    internal class ChunkSurfaceProvider
    {
        private IChunkSurfaceRepository chunkSurfaceRepository;
        private ChunkSurfaceFactory chunkSurfaceFactory;

        internal ChunkSurfaceProvider(IChunkSurfaceRepository chunkSurfaceRepository, ChunkSurfaceFactory chunkSurfaceFactory)
        {
            this.chunkSurfaceRepository = chunkSurfaceRepository;
            this.chunkSurfaceFactory = chunkSurfaceFactory;
        }

        internal ChunkSurface GetChunkSurface(ChunkGridCoordinate chunkGridCoordinate)
        {
            try
            {
                return chunkSurfaceRepository.Fetch(chunkGridCoordinate);
            }
            catch
            {
                return chunkSurfaceFactory.Create(chunkGridCoordinate);
            }
        }
    }
}