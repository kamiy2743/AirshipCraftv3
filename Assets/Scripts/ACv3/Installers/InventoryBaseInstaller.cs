using ACv3.Presentation;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Installers
{
    class InventoryBaseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InventoryService>().AsSingle();
            Container.BindInterfacesTo<InventoryInputHandler>().AsSingle();
        }
    }
}