using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using ACv3.Presentation.Players;
using Zenject;

namespace ACv3.Presentation.Rendering.Chunks
{
    public class RenderingAroundPlayer : IInitializable, IDisposable
    {
        readonly InSightChunkCreator inSightChunkCreator;
        readonly OutOfRangeChunkDisposer outOfRangeChunkDisposer;
        readonly PlayerChunkProvider playerChunkProvider;

        readonly CompositeDisposable disposals = new();
        CancellationTokenSource cts;

        const int MaxRenderingRadius = 16;

        internal RenderingAroundPlayer(InSightChunkCreator inSightChunkCreator, OutOfRangeChunkDisposer outOfRangeChunkDisposer, PlayerChunkProvider playerChunkProvider)
        {
            this.inSightChunkCreator = inSightChunkCreator;
            this.outOfRangeChunkDisposer = outOfRangeChunkDisposer;
            this.playerChunkProvider = playerChunkProvider;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .ThrottleFirstFrame(5)
                .Subscribe(_ =>
                {
                    var playerChunk = playerChunkProvider.GetPlayerChunk();

                    outOfRangeChunkDisposer.Execute(playerChunk, MaxRenderingRadius);

                    if (cts is not null)
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }

                    cts = new CancellationTokenSource();
                    inSightChunkCreator.ExecuteAsync(playerChunk, MaxRenderingRadius, cts.Token).Forget();
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}