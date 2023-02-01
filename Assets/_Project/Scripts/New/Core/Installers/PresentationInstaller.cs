using Zenject;
using Presentation;

namespace Installers
{
    internal class PresentationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<EnterWorldModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<PlaceBlockPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<BreakBlockPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<ChunkBlockUpdatePresenter>().AsSingle();
        }
    }
}