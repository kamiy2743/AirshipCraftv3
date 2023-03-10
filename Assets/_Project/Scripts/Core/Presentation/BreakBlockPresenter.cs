using System;
using UnityView.Players;
using UniRx;
using Zenject;
using UseCase;

namespace Presentation
{
    internal class BreakBlockPresenter : IInitializable, IDisposable
    {
        private BreakBlockHandler breakBlockHandler;
        private BreakBlockUseCase breakBlockUseCase;

        private CompositeDisposable disposals = new CompositeDisposable();

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