using ACv3.Presentation;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Installers
{
    class InventoryBaseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InventoryBroker>().AsSingle();
            Container.Bind<InventoryStateController>().AsSingle();
            Container.BindInterfacesTo<GrabInventoryItemService>().AsSingle();
            Container.BindInterfacesTo<InventoryInputHandler>().AsSingle();
        }
    }
}