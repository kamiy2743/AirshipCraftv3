using System;
using UnityEngine;
using Domain;
using UniRx;

namespace UnityView.Players
{
    class PlayerChunkProvider : IDisposable
    {
        readonly Subject<ChunkGridCoordinate> _onPlayerChunkChanged = new Subject<ChunkGridCoordinate>();
        internal IObservable<ChunkGridCoordinate> OnPlayerChunkChanged => _onPlayerChunkChanged;

        readonly Transform _playerTransform;
        readonly CompositeDisposable _disposals = new CompositeDisposable();

        internal PlayerChunkProvider(Transform playerTransform)
        {
            _playerTransform = playerTransform;

            var lastPlayerChunk = GetPlayerChunk();
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    var currentPlayerChunk = GetPlayerChunk();
                    if (currentPlayerChunk != lastPlayerChunk)
                    {
                        lastPlayerChunk = currentPlayerChunk;
                        _onPlayerChunkChanged.OnNext(currentPlayerChunk);
                    }
                })
                .AddTo(_disposals);
        }

        internal ChunkGridCoordinate GetPlayerChunk()
        {
            return ChunkGridCoordinate.Parse(new BlockGridCoordinate(_playerTransform.position));
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}