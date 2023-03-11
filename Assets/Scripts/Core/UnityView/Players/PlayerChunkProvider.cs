using System;
using Domain;
using UniRx;
using UnityEngine;

namespace UnityView.Players
{
    public class PlayerChunkProvider : IDisposable
    {
        readonly Subject<ChunkGridCoordinate> _onPlayerChunkChanged = new();
        internal IObservable<ChunkGridCoordinate> OnPlayerChunkChanged => _onPlayerChunkChanged;

        readonly Transform playerTransform;
        readonly CompositeDisposable disposals = new();

        public PlayerChunkProvider(Transform playerTransform)
        {
            this.playerTransform = playerTransform;

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