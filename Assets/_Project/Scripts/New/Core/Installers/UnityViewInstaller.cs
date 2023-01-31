using UnityEngine;
using Zenject;
using Infrastructure;
using UnityView.ChunkRender;
using UnityView.ChunkRender.Mesh;
using UnityView.ChunkRender.Surfaces;
using UnityView.ChunkCollision;
using UnityView.Shared;
using UnityView.Inputs;
using UnityView.Players;
using Cinemachine;

namespace Installers
{
    internal class UnityViewInstaller : MonoInstaller
    {
        [SerializeField] private ChunkRendererFactory chunkRendererFactory;
        [SerializeField] private ChunkColliderFactory chunkColliderFactory;
        [SerializeField] private CinemachineVirtualCamera playerVcam;

        public override void InstallBindings()
        {
            Container
                .Bind<IChunkSurfaceRepository>()
                .To<OnMemoryChunkSurfaceRepository>()
                .AsSingle();

            Container.Bind<ChunkSurfaceFactory>().AsSingle();
            Container.Bind<ChunkSurfaceProvider>().AsSingle();

            Container.Bind<BlockMeshDataProvider>().AsSingle();
            Container.Bind<ChunkMeshDataFactory>().AsSingle();

            Container.Bind<BlockUpdateReceptor>().AsSingle();
            Container.Bind<BlockUpdateApplier>().AsSingle();
            Container.Bind<UpdatedChunkSurfaceCalculator>().AsSingle();
            Container.Bind<ChunkRendererUpdater>().AsSingle();
            Container.BindInterfacesAndSelfTo<CreatedChunkRenderers>().AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerChunkProvider>().AsSingle();

            Container.BindInterfacesAndSelfTo<RenderingAroundPlayer>().AsSingle();
            Container.Bind<InSightChunkCreator>().AsSingle();
            Container.BindInstance<ChunkRendererFactory>(chunkRendererFactory).AsSingle();
            Container.Bind<OutOfRangeChunkDisposer>().AsSingle();

            Container.BindInterfacesAndSelfTo<AroundPlayerColliderHandler>().AsSingle();
            Container.Bind<AroundPlayerColliderCreator>().AsSingle();
            Container.Bind<OutOfRangeColliderDisposer>().AsSingle();
            Container.Bind<CreatedColliders>().AsSingle();
            Container.BindInstance<ChunkColliderFactory>(chunkColliderFactory).AsSingle();
            Container.Bind<ChunkBoundsFactory>().AsSingle();

            Container.BindInstance<PlayerCamera>(new PlayerCamera(playerVcam)).AsSingle();

            Container
                .Bind<IInputProvider>()
                .To<InputSystemInputProvider>()
                .AsSingle();
        }
    }
}