using System;
using UnityView.Players;
using UniRx;
using Zenject;
using UseCase;

namespace Presentation
{
    internal class PlaceBlockPresenter : IInitializable, IDisposable
    {
        private PlaceBlockHandler placeBlockHandler;
        private PlaceBlockUseCase placeBlockUseCase;

        private CompositeDisposable disposals = new CompositeDisposable();

        internal PlaceBlockPresenter(PlaceBlockHandler placeBlockHandler, PlaceBlockUseCase placeBlockUseCase)
        {
            this.placeBlockHandler = placeBlockHandler;
            this.placeBlockUseCase = placeBlockUseCase;
        }

        public void Initialize()
        {
            placeBlockHandler
                .OnPlaceBlock
                .Subscribe(placePosition =>
                {
                    placeBlockUseCase.PlaceBlock(placePosition, Domain.BlockTypeID.Stone);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}