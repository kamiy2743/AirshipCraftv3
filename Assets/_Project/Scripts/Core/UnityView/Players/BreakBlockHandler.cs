using System;
using UnityEngine;
using UniRx;
using Zenject;
using UnityView.Inputs;

namespace UnityView.Players
{
    public class BreakBlockHandler : IInitializable, IDisposable
    {
        private IInputProvider inputProvider;
        private FocusedBlockInfoProvider focusedBlockInfoProvider;

        private Subject<Vector3> _onBreakBlock = new Subject<Vector3>();
        public IObservable<Vector3> OnBreakBlock => _onBreakBlock;

        private CompositeDisposable disposals = new CompositeDisposable();

        private const float breakBlockInterval = 0.5f;

        internal BreakBlockHandler(IInputProvider inputProvider, FocusedBlockInfoProvider focusedBlockInfoProvider)
        {
            this.inputProvider = inputProvider;
            this.focusedBlockInfoProvider = focusedBlockInfoProvider;
        }

        public void Initialize()
        {
            var breakBlockStream = Observable.EveryUpdate().Where(_ => inputProvider.BreakBlock());
            var stopBreakBlockStream = Observable.EveryUpdate().Where(_ => !inputProvider.BreakBlock());

            FocusedBlockInfo focusedBlockInfo = null;
            breakBlockStream
                .Where(_ => focusedBlockInfoProvider.TryGetFocusedBlockInfo(out focusedBlockInfo))
                .ThrottleFirst(TimeSpan.FromSeconds(breakBlockInterval))
                .TakeUntil(stopBreakBlockStream)
                .Repeat()
                .Subscribe(_ => _onBreakBlock.OnNext(focusedBlockInfo.pivotCoordinate))
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}