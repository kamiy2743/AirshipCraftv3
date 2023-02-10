using System;
using UnityEngine;
using UniRx;
using Zenject;
using UnityView.Inputs;

namespace UnityView.Players
{
    public class PlaceBlockHandler : IInitializable, IDisposable
    {
        readonly IInputProvider _inputProvider;
        readonly FocusedBlockInfoProvider _focusedBlockInfoProvider;

        readonly Subject<Vector3> _onPlaceBlock = new Subject<Vector3>();
        public IObservable<Vector3> OnPlaceBlock => _onPlaceBlock;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        const float PlaceBlockInterval = 0.5f;

        internal PlaceBlockHandler(IInputProvider inputProvider, FocusedBlockInfoProvider focusedBlockInfoProvider)
        {
            _inputProvider = inputProvider;
            _focusedBlockInfoProvider = focusedBlockInfoProvider;
        }

        public void Initialize()
        {
            var placeBlockStream = Observable.EveryUpdate().Where(_ => _inputProvider.PlaceBlock());
            var stopPlaceBlockStream = Observable.EveryUpdate().Where(_ => !_inputProvider.PlaceBlock());

            FocusedBlockInfo focusedBlockInfo = null;
            placeBlockStream
                .Where(_ => _focusedBlockInfoProvider.TryGetFocusedBlockInfo(out focusedBlockInfo))
                .ThrottleFirst(TimeSpan.FromSeconds(PlaceBlockInterval))
                .TakeUntil(stopPlaceBlockStream)
                .Repeat()
                .Subscribe(_ =>
                {
                    var placePosition = focusedBlockInfo.HitPoint;
                    var hitNormal = focusedBlockInfo.HitNormal;
                    if (hitNormal.x < 0) placePosition.x += hitNormal.x;
                    if (hitNormal.y < 0) placePosition.y += hitNormal.y;
                    if (hitNormal.z < 0) placePosition.z += hitNormal.z;

                    _onPlaceBlock.OnNext(placePosition);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}