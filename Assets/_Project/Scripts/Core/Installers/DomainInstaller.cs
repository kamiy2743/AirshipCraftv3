using Zenject;
using Domain.Chunks;
using Infrastructure;

namespace Installers
{
    internal class DomainInstaller : MonoInstaller
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