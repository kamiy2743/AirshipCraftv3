using UnityEngine;
using Zenject;
using Infrastructure;
using UnityView.Rendering;
using UnityView.Rendering.Chunks;
using UnityView.ChunkCollision;
using UnityView.Inputs;
using UnityView.Players;

namespace Installers
{
    class UnityViewInstaller : MonoInstaller
    {
        [SerializeField] ChunkRendererFactory chunkRendererFactory;
        [SerializeField] ChunkColliderFactory chunkColliderFactory;
        [SerializeField] Transform playerTransform;
        [SerializeField] PlayerCamera playerCamera;
        [SerializeField] FocusedBlockOutline focusedBlockOutline;
        [SerializeField] Material blockMaterial;

        public override void InstallBindings()
        {
            Container
                .Bind<IChunkSurfaceRepository>()
                .To<OnMemoryChunkSurfaceRepository>()
                .AsSingle();

            Container.Bind<ChunkSurfaceFactory>().AsSingle();
            Container.Bind<ChunkSurfaceProvider>().AsSingle();

            Container.Bind<BlockMeshProvider>().AsSingle();
            Container.Bind<BlockMeshFactory>().AsSingle();
            Container.Bind<ChunkMeshFactory>().AsSingle();

            Container.Bind<UnityView.Rendering.Chunks.BlockUpdateApplier>().AsSingle();
            Container.Bind<UpdatedChunkSurfaceCalculator>().AsSingle();
            Container.Bind<ChunkRendererUpdater>().AsSingle();
            Container.BindInterfacesAndSelfTo<CreatedChunkRenderers>().AsSingle();

            Container
                .BindInterfacesAndSelfTo<PlayerChunkProvider>()
                .FromInstance(new PlayerChunkProvider(playerTransform))
                .AsSingle();

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

            // TODO Installerを分けたい
            Container.Bind<UnityView.ChunkCollision.BlockUpdateApplier>().AsSingle();
            Container.Bind<UpdatedChunkBoundsCalculator>().AsSingle();
            Container.Bind<ChunkColliderUpdater>().AsSingle();

            Container.BindInstance<PlayerCamera>(playerCamera).AsSingle();

            Container
                .Bind<IInputProvider>()
                .To<InputSystemInputProvider>()
                .AsSingle();

            Container.Bind<FocusedBlockInfoProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<FocusedBlockViewer>().AsSingle();
            Container.BindInstance<FocusedBlockOutline>(focusedBlockOutline).AsSingle();

            Container.BindInterfacesAndSelfTo<PlaceBlockHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<BreakBlockHandler>().AsSingle();

            Container.BindInterfacesAndSelfTo<SixFaceTextureAtlas>().AsSingle();
            Container.Bind<SixFaceUVCreator>().AsSingle();

            Container.BindInterfacesAndSelfTo<TextureAtlasApplier>().AsSingle().WithArguments(blockMaterial);
        }
    }
}