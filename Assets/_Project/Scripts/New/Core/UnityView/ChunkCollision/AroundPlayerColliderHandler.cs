using System;
using Zenject;
using UnityView.Shared;
using UniRx;

namespace UnityView.ChunkCollision
{
    internal class AroundPlayerColliderHandler : IInitializable, IDisposable
    {
        private AroundPlayerColliderCreator aroundPlayerColliderCreator;
        private OutOfRangeColliderDisposer outOfRangeColliderDisposer;
        private PlayerChunkProvider playerChunkProvider;

        private CompositeDisposable disposals = new CompositeDisposable();

        private const int ColliderRadius = 1;

        internal AroundPlayerColliderHandler(AroundPlayerColliderCreator aroundPlayerColliderCreator, OutOfRangeColliderDisposer outOfRangeColliderDisposer, PlayerChunkProvider playerChunkProvider)
        {
            this.aroundPlayerColliderCreator = aroundPlayerColliderCreator;
            this.outOfRangeColliderDisposer = outOfRangeColliderDisposer;
            this.playerChunkProvider = playerChunkProvider;
        }

        public void Initialize()
        {
            aroundPlayerColliderCreator.Execute(playerChunkProvider.GetPlayerChunk(), ColliderRadius);

            playerChunkProvider
                .OnPlayerChunkChanged
                .Subscribe(playerChunk =>
                {
                    outOfRangeColliderDisposer.Execute(playerChunk, ColliderRadius);
                    aroundPlayerColliderCreator.Execute(playerChunk, ColliderRadius);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}