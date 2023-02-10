using System;
using Zenject;
using UnityView.Players;
using UniRx;

namespace UnityView.ChunkCollision
{
    class AroundPlayerColliderHandler : IInitializable, IDisposable
    {
        readonly AroundPlayerColliderCreator _aroundPlayerColliderCreator;
        readonly OutOfRangeColliderDisposer _outOfRangeColliderDisposer;
        readonly PlayerChunkProvider _playerChunkProvider;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        const int ColliderRadius = 1;

        internal AroundPlayerColliderHandler(AroundPlayerColliderCreator aroundPlayerColliderCreator, OutOfRangeColliderDisposer outOfRangeColliderDisposer, PlayerChunkProvider playerChunkProvider)
        {
            _aroundPlayerColliderCreator = aroundPlayerColliderCreator;
            _outOfRangeColliderDisposer = outOfRangeColliderDisposer;
            _playerChunkProvider = playerChunkProvider;
        }

        public void Initialize()
        {
            _aroundPlayerColliderCreator.Execute(_playerChunkProvider.GetPlayerChunk(), ColliderRadius);

            _playerChunkProvider
                .OnPlayerChunkChanged
                .Subscribe(playerChunk =>
                {
                    _outOfRangeColliderDisposer.Execute(playerChunk, ColliderRadius);
                    _aroundPlayerColliderCreator.Execute(playerChunk, ColliderRadius);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}