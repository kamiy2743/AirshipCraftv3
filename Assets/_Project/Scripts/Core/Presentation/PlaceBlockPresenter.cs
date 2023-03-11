using System;
using Domain;
using UniRx;
using UnityView.Players;
using UseCase;
using Zenject;

namespace Presentation
{
    class PlaceBlockPresenter : IInitializable, IDisposable
    {
        readonly PlaceBlockHandler placeBlockHandler;
        readonly PlaceBlockUseCase placeBlockUseCase;

        readonly CompositeDisposable disposals = new();

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
                    placeBlockUseCase.PlaceBlock(placePosition, BlockType.Stone);
                })
                .AddTo(disposals);
        }

        public void Dispose()
        {
            disposals.Dispose();
        }
    }
}