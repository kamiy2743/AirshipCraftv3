using Domain;
using UnityView.ChunkRendering.Model.ChunkMesh;
using UnityView.ChunkRendering.Model.RenderingSurface;

namespace UnityView.ChunkRendering
{
    internal class BlockUpdateApplier
    {
        private UpdatedChunkRenderingSurfaceCalculator updatedChunkRenderingSurfaceCalculator;
        private ChunkRendererUpdater chunkRendererUpdater;
        private ChunkMeshFactory chunkMeshFactory;
        private IChunkRenderingSurfaceRepository chunkRenderingSurfaceRepository;

        internal BlockUpdateApplier(UpdatedChunkRenderingSurfaceCalculator updatedChunkRenderingSurfaceCalculator, ChunkRendererUpdater chunkRendererUpdater, ChunkMeshFactory chunkMeshFactory, IChunkRenderingSurfaceRepository chunkRenderingSurfaceRepository)
        {
            this.updatedChunkRenderingSurfaceCalculator = updatedChunkRenderingSurfaceCalculator;
            this.chunkRendererUpdater = chunkRendererUpdater;
            this.chunkMeshFactory = chunkMeshFactory;
            this.chunkRenderingSurfaceRepository = chunkRenderingSurfaceRepository;
        }

        internal void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkRenderingSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var renderingSurface in results)
            {
                chunkRenderingSurfaceRepository.Store(renderingSurface);

                var cgc = renderingSurface.chunkGridCoordinate;
                var mesh = chunkMeshFactory.Create(cgc);
                chunkRendererUpdater.Update(cgc, mesh);
            }
        }
    }
}