using MasterData;
using UnityView.Rendering;
using Zenject;

namespace Installers
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