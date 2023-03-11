using System;
using UniRx;
using UnityView.Players;
using UseCase;
using Zenject;

namespace Presentation
{
    class BreakBlockPresenter : IInitializable, IDisposable
    {
        readonly BreakBlockHandler breakBlockHandler;
        readonly BreakBlockUseCase breakBlockUseCase;

        readonly CompositeDisposable disposals = new();

        internal BreakBlockPresenter(BreakBlockHandler breakBlockHandler, BreakBlockUseCase breakBlockUseCase)
        {
            this.breakBlockHandler = breakBlockHandler;
            this.breakBlockUseCase = breakBlockUseCase;
        }

        public void Initialize()
        {
            breakBlockHandler
                .OnBreakBlock
                .Subscribe(breakPosition =>
                {
                    breakBlockUseCase.BreakBlock(breakPosition);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}