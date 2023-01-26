using System;
using UnityEngine;
using UniRx;
using Zenject;
using Domain;
using Domain.Chunks;

namespace UnityView.ChunkRendering
{
    internal class RenderingAroundPlayer : IInitializable, IDisposable
    {
        private InSightChunkCreator inSightChunkCreator;
        private OutOfRangeChunkDisposer outOfRangeChunkRemover;

        private Transform cameraTransform;
        private CompositeDisposable disposals = new CompositeDisposable();

        private const int MaxRenderingRadius = 1;

        internal RenderingAroundPlayer(InSightChunkCreator inSightChunkCreator, OutOfRangeChunkDisposer outOfRangeChunkRemover)
        {
            this.inSightChunkCreator = inSightChunkCreator;
            this.outOfRangeChunkRemover = outOfRangeChunkRemover;

            cameraTransform = Camera.main.transform;
        }

        public void Initialize()
        {

            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    var playerChunk = ChunkGridCoordinate.Parse(new BlockGridCoordinate(cameraTransform.position));
                    inSightChunkCreator.Execute(playerChunk, MaxRenderingRadius);
                    outOfRangeChunkRemover.Execute(playerChunk, MaxRenderingRadius);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}