using ACv3.MasterData;
using ACv3.UnityView.Rendering;
using Zenject;

namespace ACv3.Installers
{
    class MasterDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IBlockShapeMeshProvider>()
                .To<BlockShapeMeshProvider>()
                .AsSingle();

            Container
                .Bind<ISixFaceTextureProvider>()
                .To<SixFaceTextureProvider>()
                .AsSingle();
        }
    }
}