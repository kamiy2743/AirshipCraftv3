using System;
using System.Threading;
using UnityEngine;
using UniRx;
using Zenject;
using Domain;
using UnityView.Players;
using Cysharp.Threading.Tasks;

namespace UnityView.Rendering.Chunks
{
    class RenderingAroundPlayer : IInitializable, IDisposable
    {
        readonly InSightChunkCreator _inSightChunkCreator;
        readonly OutOfRangeChunkDisposer _outOfRangeChunkDisposer;
        readonly PlayerChunkProvider _playerChunkProvider;

        readonly CompositeDisposable _disposals = new CompositeDisposable();
        CancellationTokenSource _cts;

        const int MaxRenderingRadius = 16;

        internal RenderingAroundPlayer(InSightChunkCreator inSightChunkCreator, OutOfRangeChunkDisposer outOfRangeChunkDisposer, PlayerChunkProvider playerChunkProvider)
        {
            _inSightChunkCreator = inSightChunkCreator;
            _outOfRangeChunkDisposer = outOfRangeChunkDisposer;
            _playerChunkProvider = playerChunkProvider;
        }

        public void Initialize()
        {
            Observable
                .EveryUpdate()
                .ThrottleFirstFrame(5)
                .Subscribe(_ =>
                {
                    var playerChunk = _playerChunkProvider.GetPlayerChunk();

                    _outOfRangeChunkDisposer.Execute(playerChunk, MaxRenderingRadius);

                    if (_cts is not null)
                    {
                        _cts.Cancel();
                        _cts.Dispose();
                    }

                    _cts = new CancellationTokenSource();
                    _inSightChunkCreator.ExecuteAsync(playerChunk, MaxRenderingRadius, _cts.Token).Forget();
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}