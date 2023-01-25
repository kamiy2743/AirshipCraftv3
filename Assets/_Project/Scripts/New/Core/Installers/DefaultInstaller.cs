using Zenject;
using Domain.Chunks;
using Infrastructure;
using UseCase;
using Presentation;
using RenderingDomain;
using RenderingDomain.RenderingSurface;
using RenderingDomain.ChunkMesh;

namespace Installers
{
    public class DefaultInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IChunkRepository>()
                .To<OnMemoryChunkRepository>()
                .AsCached();

            Container
                .Bind<IChunkFactory>()
                .To<AllDirtChunkFactory>()
                .AsCached();

            Container
                .Bind<IChunkProvider>()
                .To<ChunkProvider>()
                .AsCached();

            Container.Bind<PlaceBlockUseCase>().AsCached();

            Container.Bind<SceneLoader>().AsCached();
            Container.Bind<EnterWorldUseCase>().AsCached();
            Container.Bind<EnterWorldModel>().AsCached();

            Container
                .Bind<IChunkRenderingSurfaceRepository>()
                .To<OnMemoryChunkRenderingSurfaceRepository>()
                .AsCached();

            Container
                .Bind<IChunkRenderingSurfaceFactory>()
                .To<ChunkRenderingSurfaceFactory>()
                .AsCached();

            Container
                .Bind<IChunkRenderingSurfaceProvider>()
                .To<ChunkRenderingSurfaceProvider>()
                .AsCached();

            Container
                .Bind<IBlockMeshProvider>()
                .To<BlockMeshProvider>()
                .AsCached();

            Container.Bind<ChunkMeshFactory>().AsCached();
        }
    }
}