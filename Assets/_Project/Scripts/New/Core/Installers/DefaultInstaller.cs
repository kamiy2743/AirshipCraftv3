using Zenject;
using Domain.Chunks;
using Infrastructure;
using UseCase;

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
                .To<ChunkFactory>()
                .AsCached();

            Container
                .Bind<IChunkProvider>()
                .To<ChunkProvider>()
                .AsCached();

            Container.Bind<SetBlockService>().AsCached();
            Container.Bind<PlaceBlockUseCase>().AsCached();
        }
    }
}