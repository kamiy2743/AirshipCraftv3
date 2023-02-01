using Domain;

namespace UnityView.Render.Chunks
{
    public class BlockUpdateApplier
    {
        private UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator;
        private ChunkRendererUpdater chunkRendererUpdater;
        private ChunkMeshFactory chunkMeshFactory;
        private IChunkSurfaceRepository chunkSurfaceRepository;

        internal BlockUpdateApplier(UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator, ChunkRendererUpdater chunkRendererUpdater, ChunkMeshFactory chunkMeshFactory, IChunkSurfaceRepository chunkSurfaceRepository)
        {
            this.updatedChunkSurfaceCalculator = updatedChunkSurfaceCalculator;
            this.chunkRendererUpdater = chunkRendererUpdater;
            this.chunkMeshFactory = chunkMeshFactory;
            this.chunkSurfaceRepository = chunkSurfaceRepository;
        }

        public void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var chunkSurface in results)
            {
                chunkSurfaceRepository.Store(chunkSurface);

                var mesh = chunkMeshFactory.Create(chunkSurface.chunkGridCoordinate);
                chunkRendererUpdater.Update(mesh);
            }
        }
    }
}