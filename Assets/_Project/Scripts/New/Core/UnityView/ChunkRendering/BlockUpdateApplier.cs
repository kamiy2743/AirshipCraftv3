using Domain;
using UnityView.ChunkRendering.Mesh;
using UnityView.ChunkRendering.RenderingSurface;

namespace UnityView.ChunkRendering
{
    internal class BlockUpdateApplier
    {
        private UpdatedChunkRenderingSurfaceCalculator updatedChunkRenderingSurfaceCalculator;
        private ChunkRendererUpdater chunkRendererUpdater;
        private ChunkMeshDataFactory chunkMeshDataFactory;
        private IChunkRenderingSurfaceRepository chunkRenderingSurfaceRepository;

        internal BlockUpdateApplier(UpdatedChunkRenderingSurfaceCalculator updatedChunkRenderingSurfaceCalculator, ChunkRendererUpdater chunkRendererUpdater, ChunkMeshDataFactory chunkMeshDataFactory, IChunkRenderingSurfaceRepository chunkRenderingSurfaceRepository)
        {
            this.updatedChunkRenderingSurfaceCalculator = updatedChunkRenderingSurfaceCalculator;
            this.chunkRendererUpdater = chunkRendererUpdater;
            this.chunkMeshDataFactory = chunkMeshDataFactory;
            this.chunkRenderingSurfaceRepository = chunkRenderingSurfaceRepository;
        }

        internal void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = updatedChunkRenderingSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var renderingSurface in results)
            {
                chunkRenderingSurfaceRepository.Store(renderingSurface);

                var cgc = renderingSurface.chunkGridCoordinate;
                var mesh = chunkMeshDataFactory.Create(cgc);
                chunkRendererUpdater.Update(cgc, mesh);
            }
        }
    }
}