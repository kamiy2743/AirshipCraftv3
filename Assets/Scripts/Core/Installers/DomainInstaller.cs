using ACv3.Domain.Chunks;
using ACv3.Infrastructure;
using Zenject;

namespace ACv3.Installers
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