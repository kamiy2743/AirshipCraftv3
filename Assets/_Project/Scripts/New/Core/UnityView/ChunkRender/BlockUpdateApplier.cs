using Domain;
using UnityView.ChunkRender.Mesh;
using UnityView.ChunkRender.RenderingSurface;

namespace UnityView.ChunkRender
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