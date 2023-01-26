using Zenject;
using Domain.Chunks;
using Infrastructure;
using UseCase;
using Presentation;
using UnityView.ChunkRendering;
using UnityView.ChunkRendering.Model;
using UnityView.ChunkRendering.Model.ChunkMesh;
using UnityView.ChunkRendering.Model.RenderingSurface;
using UnityView.ChunkRendering.View;

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

            Container.Bind<BlockUpdateReceptor>().AsCached();
            Container.Bind<BlockUpdateApplier>().AsCached();
            Container.Bind<UpdatedChunkRenderingSurfaceCalculator>().AsCached();
            Container.Bind<ChunkRendererUpdater>().AsCached();
            Container.Bind<CreatedChunkRenderers>().AsCached();
        }
    }
}