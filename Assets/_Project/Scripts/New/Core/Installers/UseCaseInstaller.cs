using Zenject;
using UseCase;

namespace Installers
{
    internal class UseCaseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlaceBlockUseCase>().AsSingle();
            Container.Bind<BreakBlockUseCase>().AsSingle();
            Container.Bind<EnterWorldUseCase>().AsSingle();
        }
    }
}