using System;
using UnityView.Players;
using UniRx;
using Zenject;
using UseCase;

namespace Presentation
{
    class BreakBlockPresenter : IInitializable, IDisposable
    {
        readonly BreakBlockHandler _breakBlockHandler;
        readonly BreakBlockUseCase _breakBlockUseCase;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        internal BreakBlockPresenter(BreakBlockHandler breakBlockHandler, BreakBlockUseCase breakBlockUseCase)
        {
            _breakBlockHandler = breakBlockHandler;
            _breakBlockUseCase = breakBlockUseCase;
        }

        public void Initialize()
        {
            _breakBlockHandler
                .OnBreakBlock
                .Subscribe(breakPosition =>
                {
                    _breakBlockUseCase.BreakBlock(breakPosition);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}