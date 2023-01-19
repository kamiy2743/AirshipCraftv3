using Zenject;
using Domain.Chunks;
using Infrastructure;
using UseCase;
using Presentation;
// using 

namespace Installers
{
    public class DefaultInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IChunkRepository>()
                .To<OnMemoryChunkRepository>()
                .AsCached();

            Container
                .Bind<IChunkFactory>()
                .To<AllDirtChunkFactory>()
                .AsCached();

            Container
                .Bind<IChunkProvider>()
                .To<ChunkProvider>()
                .AsCached();

            Container.Bind<PlaceBlockUseCase>().AsCached();
            // Container.Bind <

            Container.Bind<SceneLoader>().AsCached();
            Container.Bind<EnterWorldUseCase>().AsCached();
            Container.Bind<EnterWorldModel>().AsCached();
        }
    }
}