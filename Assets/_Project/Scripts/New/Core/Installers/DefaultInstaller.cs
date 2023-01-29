using Zenject;
using Domain.Chunks;
using Infrastructure;
using UseCase;
using Presentation;
using UnityView.ChunkRendering;
using UnityView.ChunkRendering.Mesh;
using UnityView.ChunkRendering.RenderingSurface;
using UnityEngine;

namespace Installers
{
    public class DefaultInstaller : MonoInstaller
    {
        [SerializeField] private ChunkRendererFactory chunkRendererFactory;

        public override void InstallBindings()
        {
            Container
                .Bind<IChunkRepository>()
                .To<OnMemoryChunkRepository>()
                .AsCached();

            Container
                .Bind<IChunkFactory>()
                .To<SnoiseChunkFactory>()
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

            Container.Bind<ChunkRenderingSurfaceFactory>().AsCached();
            Container.Bind<ChunkRenderingSurfaceProvider>().AsCached();

            Container.Bind<BlockMeshProvider>().AsCached();
            Container.Bind<ChunkMeshDataFactory>().AsCached();

            Container.Bind<BlockUpdateReceptor>().AsCached();
            Container.Bind<BlockUpdateApplier>().AsCached();
            Container.Bind<UpdatedChunkRenderingSurfaceCalculator>().AsCached();
            Container.Bind<ChunkRendererUpdater>().AsCached();
            Container.BindInterfacesAndSelfTo<CreatedChunkRenderers>().AsCached();

            Container.BindInterfacesAndSelfTo<RenderingAroundPlayer>().AsCached();
            Container.Bind<InSightChunkCreator>().AsCached();
            Container.BindInstance<ChunkRendererFactory>(chunkRendererFactory).AsCached();
            Container.Bind<InSightChecker>().AsCached();
            Container.Bind<OutOfRangeChunkDisposer>().AsCached();
        }
    }
}