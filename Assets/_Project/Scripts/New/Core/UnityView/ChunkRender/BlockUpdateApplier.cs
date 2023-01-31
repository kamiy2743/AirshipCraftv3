using Domain;
using UnityView.ChunkRender;

namespace UnityView.ChunkRender
{
    internal class BlockUpdateApplier
    {
        private UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator;
        private ChunkRendererUpdater chunkRendererUpdater;
        private ChunkMeshDataFactory chunkMeshDataFactory;
        private IChunkSurfaceRepository chunkSurfaceRepository;

        internal BlockUpdateApplier(UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator, ChunkRendererUpdater chunkRendererUpdater, ChunkMeshDataFactory chunkMeshDataFactory, IChunkSurfaceRepository chunkSurfaceRepository)
        {
            this.updatedChunkSurfaceCalculator = updatedChunkSurfaceCalculator;
            this.chunkRendererUpdater = chunkRendererUpdater;
            this.chunkMeshDataFactory = chunkMeshDataFactory;
            this.chunkSurfaceRepository = chunkSurfaceRepository;
        }

        internal void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var renderingSurface in results)
            {
                chunkSurfaceRepository.Store(renderingSurface);

                var cgc = renderingSurface.chunkGridCoordinate;
                var mesh = chunkMeshDataFactory.Create(cgc);
                chunkRendererUpdater.Update(cgc, mesh);
            }
        }
    }
}