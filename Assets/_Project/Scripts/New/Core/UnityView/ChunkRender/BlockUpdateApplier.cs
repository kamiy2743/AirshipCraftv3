using Domain;
using UnityView.ChunkRender;

namespace UnityView.ChunkRender
{
    internal class BlockUpdateApplier
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

        internal void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var renderingSurface in results)
            {
                chunkSurfaceRepository.Store(renderingSurface);

                var cgc = renderingSurface.chunkGridCoordinate;
                var mesh = chunkMeshFactory.Create(cgc);
                chunkRendererUpdater.Update(cgc, mesh);
            }
        }
    }
}