using Domain.Chunks;
using Infrastructure;
using Zenject;

namespace Installers
{
    class DomainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IChunkRepository>()
                .To<OnMemoryChunkRepository>()
                .AsSingle();

            Container
                .Bind<IChunkFactory>()
                .To<SnoiseChunkFactory>()
                .AsSingle();

            Container
                .Bind<IChunkProvider>()
                .To<ChunkProvider>()
                .AsSingle();
        }
    }
}