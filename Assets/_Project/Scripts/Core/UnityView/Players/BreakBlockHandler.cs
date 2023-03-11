using System;
using UniRx;
using UnityEngine;
using UnityView.Inputs;
using Zenject;

namespace UnityView.Players
{
    public class BreakBlockHandler : IInitializable, IDisposable
    {
        readonly IInputProvider inputProvider;
        readonly FocusedBlockInfoProvider focusedBlockInfoProvider;

        readonly Subject<Vector3> _onBreakBlock = new();
        public IObservable<Vector3> OnBreakBlock => _onBreakBlock;

        readonly CompositeDisposable disposals = new();

        const float breakBlockInterval = 0.5f;

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