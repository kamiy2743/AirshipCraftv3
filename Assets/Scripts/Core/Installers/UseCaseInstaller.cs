using ACv3.UseCase;
using Zenject;

namespace ACv3.Installers
{
    class UseCaseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlaceBlockUseCase>().AsSingle();
            Container.Bind<BreakBlockUseCase>().AsSingle();
            Container.Bind<ChunkBlockSetter>().AsSingle();

            Container.Bind<EnterWorldUseCase>().AsSingle();
        }
    }
}