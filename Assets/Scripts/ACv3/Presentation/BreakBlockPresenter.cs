using System;
using UniRx;
using ACv3.UnityView.Players;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Presentation
{
    public class BreakBlockPresenter : IInitializable, IDisposable
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