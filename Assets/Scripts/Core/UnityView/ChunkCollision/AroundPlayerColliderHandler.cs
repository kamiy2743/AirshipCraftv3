using System;
using UniRx;
using UnityView.Players;
using Zenject;

namespace UnityView.ChunkCollision
{
    public class AroundPlayerColliderHandler : IInitializable, IDisposable
    {
        readonly AroundPlayerColliderCreator aroundPlayerColliderCreator;
        readonly OutOfRangeColliderDisposer outOfRangeColliderDisposer;
        readonly PlayerChunkProvider playerChunkProvider;

        readonly CompositeDisposable disposals = new();

        const int ColliderRadius = 1;

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