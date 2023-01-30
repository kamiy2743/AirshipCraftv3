using System;
using System.Threading;
using UnityEngine;
using UniRx;
using Zenject;
using Domain;
using Cysharp.Threading.Tasks;

namespace UnityView.ChunkRendering
{
    internal class RenderingAroundPlayer : IInitializable, IDisposable
    {
        private InSightChunkCreator inSightChunkCreator;
        private OutOfRangeChunkDisposer outOfRangeChunkDisposer;

        private Transform cameraTransform;
        private CompositeDisposable disposals = new CompositeDisposable();
        private CancellationTokenSource cts;

        private const int MaxRenderingRadius = 16;

        internal RenderingAroundPlayer(InSightChunkCreator inSightChunkCreator, OutOfRangeChunkDisposer outOfRangeChunkDisposer)
        {
            this.inSightChunkCreator = inSightChunkCreator;
            this.outOfRangeChunkDisposer = outOfRangeChunkDisposer;

            cameraTransform = Camera.main.transform;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .ThrottleFirstFrame(5)
                .Subscribe(_ =>
                {
                    var playerChunk = ChunkGridCoordinate.Parse(new BlockGridCoordinate(cameraTransform.position));

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