using UnityEngine;
using Zenject;
using Infrastructure;
using UnityView.ChunkRendering;
using UnityView.ChunkRendering.Mesh;
using UnityView.ChunkRendering.RenderingSurface;
using UnityView.Inputs;

namespace Installers
{
    internal class UnityViewInstaller : MonoInstaller
    {
        [SerializeField] private ChunkRendererFactory chunkRendererFactory;

        public override void InstallBindings()
        {
            Container
                .Bind<IChunkRenderingSurfaceRepository>()
                .To<OnMemoryChunkRenderingSurfaceRepository>()
                .AsSingle();

            Container.Bind<ChunkRenderingSurfaceFactory>().AsSingle();
            Container.Bind<ChunkRenderingSurfaceProvider>().AsSingle();

            Container.Bind<BlockMeshDataProvider>().AsSingle();
            Container.Bind<ChunkMeshDataFactory>().AsSingle();

            Container.Bind<BlockUpdateReceptor>().AsSingle();
            Container.Bind<BlockUpdateApplier>().AsSingle();
            Container.Bind<UpdatedChunkRenderingSurfaceCalculator>().AsSingle();
            Container.Bind<ChunkRendererUpdater>().AsSingle();
            Container.BindInterfacesAndSelfTo<CreatedChunkRenderers>().AsSingle();

            Container.BindInterfacesAndSelfTo<RenderingAroundPlayer>().AsSingle();
            Container.Bind<InSightChunkCreator>().AsSingle();
            Container.BindInstance<ChunkRendererFactory>(chunkRendererFactory).AsSingle();
            Container.Bind<InSightChecker>().AsSingle();
            Container.Bind<OutOfRangeChunkDisposer>().AsSingle();

            Container
                .Bind<IInputProvider>()
                .To<InputSystemInputProvider>()
                .AsSingle();
        }
    }
}