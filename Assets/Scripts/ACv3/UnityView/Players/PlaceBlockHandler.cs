using System;
using UniRx;
using UnityEngine;
using ACv3.UnityView.Inputs;
using Zenject;

namespace ACv3.UnityView.Players
{
    public class PlaceBlockHandler : IInitializable, IDisposable
    {
        readonly IInputController inputController;
        readonly FocusedBlockInfoProvider focusedBlockInfoProvider;

        readonly Subject<Vector3> _onPlaceBlock = new();
        public IObservable<Vector3> OnPlaceBlock => _onPlaceBlock;

        readonly CompositeDisposable disposals = new();

        const float placeBlockInterval = 0.5f;

        internal PlaceBlockHandler(IInputController inputController, FocusedBlockInfoProvider focusedBlockInfoProvider)
        {
            this.inputController = inputController;
            this.focusedBlockInfoProvider = focusedBlockInfoProvider;
        }

        public void Initialize()
        {
            var placeBlockStream = Observable.EveryUpdate().Where(_ => inputController.PlaceBlock());
            var stopPlaceBlockStream = Observable.EveryUpdate().Where(_ => !inputController.PlaceBlock());

            FocusedBlockInfo focusedBlockInfo = null;
            placeBlockStream
                .Where(_ => focusedBlockInfoProvider.TryGetFocusedBlockInfo(out focusedBlockInfo))
                .ThrottleFirst(TimeSpan.FromSeconds(placeBlockInterval))
                .TakeUntil(stopPlaceBlockStream)
                .Repeat()
                .Subscribe(_ =>
                {
                    var placePosition = focusedBlockInfo.hitPoint;
                    var hitNormal = focusedBlockInfo.hitNormal;
                    if (hitNormal.x < 0) placePosition.x += hitNormal.x;
                    if (hitNormal.y < 0) placePosition.y += hitNormal.y;
                    if (hitNormal.z < 0) placePosition.z += hitNormal.z;

                    _onPlaceBlock.OnNext(placePosition);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}