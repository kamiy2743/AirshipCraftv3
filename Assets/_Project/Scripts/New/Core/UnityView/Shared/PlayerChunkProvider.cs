using System;
using UnityEngine;
using Domain;
using UniRx;
namespace UnityView.Shared
{
    internal class PlayerChunkProvider : IDisposable
    {
        private Subject<ChunkGridCoordinate> _onPlayerChunkChanged = new Subject<ChunkGridCoordinate>();
        internal IObservable<ChunkGridCoordinate> OnPlayerChunkChanged => _onPlayerChunkChanged;

        private Transform playerTransform;
        private CompositeDisposable disposals = new CompositeDisposable();

        internal PlayerChunkProvider()
        {
            playerTransform = Camera.main.transform;

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
                .AddTo(disposals);
        }

        internal ChunkGridCoordinate GetPlayerChunk()
        {
            return ChunkGridCoordinate.Parse(new BlockGridCoordinate(playerTransform.position));
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}