using System;
using UniRx;
using UnityEngine;
using ACv3.Presentation.Inputs;
using Zenject;

namespace ACv3.Presentation.Players
{
    public class BreakBlockHandler : IInitializable, IDisposable
    {
        readonly IInputController inputController;
        readonly FocusedBlockInfoProvider focusedBlockInfoProvider;

        readonly Subject<Vector3> _onBreakBlock = new();
        public IObservable<Vector3> OnBreakBlock => _onBreakBlock;

        readonly CompositeDisposable disposals = new();

        const float breakBlockInterval = 0.5f;

        internal BreakBlockHandler(IInputController inputController, FocusedBlockInfoProvider focusedBlockInfoProvider)
        {
            this.inputController = inputController;
            this.focusedBlockInfoProvider = focusedBlockInfoProvider;
        }

        public void Initialize()
        {
            var breakBlockStream = Observable.EveryUpdate().Where(_ => inputController.BreakBlock());
            var stopBreakBlockStream = Observable.EveryUpdate().Where(_ => !inputController.BreakBlock());

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