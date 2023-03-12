using ACv3.Domain;

namespace ACv3.UnityView.Rendering.Chunks
{
    public class BlockUpdateApplier
    {
        readonly UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator;
        readonly ChunkRendererUpdater chunkRendererUpdater;
        readonly ChunkMeshFactory chunkMeshFactory;
        readonly IChunkSurfaceRepository chunkSurfaceRepository;

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