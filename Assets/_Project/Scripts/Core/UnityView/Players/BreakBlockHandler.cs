using System;
using UnityEngine;
using UniRx;
using Zenject;
using UnityView.Inputs;

namespace UnityView.Players
{
    public class BreakBlockHandler : IInitializable, IDisposable
    {
        readonly IInputProvider _inputProvider;
        readonly FocusedBlockInfoProvider _focusedBlockInfoProvider;

        readonly Subject<Vector3> _onBreakBlock = new Subject<Vector3>();
        public IObservable<Vector3> OnBreakBlock => _onBreakBlock;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        const float BreakBlockInterval = 0.5f;

        internal BreakBlockHandler(IInputProvider inputProvider, FocusedBlockInfoProvider focusedBlockInfoProvider)
        {
            _inputProvider = inputProvider;
            _focusedBlockInfoProvider = focusedBlockInfoProvider;
        }

        public void Initialize()
        {
            var breakBlockStream = Observable.EveryUpdate().Where(_ => _inputProvider.BreakBlock());
            var stopBreakBlockStream = Observable.EveryUpdate().Where(_ => !_inputProvider.BreakBlock());

            FocusedBlockInfo focusedBlockInfo = null;
            breakBlockStream
                .Where(_ => _focusedBlockInfoProvider.TryGetFocusedBlockInfo(out focusedBlockInfo))
                .ThrottleFirst(TimeSpan.FromSeconds(BreakBlockInterval))
                .TakeUntil(stopBreakBlockStream)
                .Repeat()
                .Subscribe(_ => _onBreakBlock.OnNext(focusedBlockInfo.PivotCoordinate))
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}