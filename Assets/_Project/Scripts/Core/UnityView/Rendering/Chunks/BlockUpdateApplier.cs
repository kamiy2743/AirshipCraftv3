using Domain;

namespace UnityView.Rendering.Chunks
{
    public class BlockUpdateApplier
    {
        readonly UpdatedChunkSurfaceCalculator _updatedChunkSurfaceCalculator;
        readonly ChunkRendererUpdater _chunkRendererUpdater;
        readonly ChunkMeshFactory _chunkMeshFactory;
        readonly IChunkSurfaceRepository _chunkSurfaceRepository;

        internal BlockUpdateApplier(UpdatedChunkSurfaceCalculator updatedChunkSurfaceCalculator, ChunkRendererUpdater chunkRendererUpdater, ChunkMeshFactory chunkMeshFactory, IChunkSurfaceRepository chunkSurfaceRepository)
        {
            _updatedChunkSurfaceCalculator = updatedChunkSurfaceCalculator;
            _chunkRendererUpdater = chunkRendererUpdater;
            _chunkMeshFactory = chunkMeshFactory;
            _chunkSurfaceRepository = chunkSurfaceRepository;
        }

        public void Apply(BlockGridCoordinate updateCoordinate)
        {
            var results = _updatedChunkSurfaceCalculator.Calculate(updateCoordinate);

            foreach (var chunkSurface in results)
            {
                _chunkSurfaceRepository.Store(chunkSurface);

                var mesh = _chunkMeshFactory.Create(chunkSurface.ChunkGridCoordinate);
                _chunkRendererUpdater.Update(mesh);
            }
        }
    }
}