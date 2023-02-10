using System;
using UnityView.Players;
using UniRx;
using Zenject;
using UseCase;

namespace Presentation
{
    class PlaceBlockPresenter : IInitializable, IDisposable
    {
        readonly PlaceBlockHandler _placeBlockHandler;
        readonly PlaceBlockUseCase _placeBlockUseCase;

        readonly CompositeDisposable _disposals = new CompositeDisposable();

        internal PlaceBlockPresenter(PlaceBlockHandler placeBlockHandler, PlaceBlockUseCase placeBlockUseCase)
        {
            _placeBlockHandler = placeBlockHandler;
            _placeBlockUseCase = placeBlockUseCase;
        }

        public void Initialize()
        {
            _placeBlockHandler
                .OnPlaceBlock
                .Subscribe(placePosition =>
                {
                    _placeBlockUseCase.PlaceBlock(placePosition, Domain.BlockType.Stone);
                })
                .AddTo(_disposals);
        }

        public void Dispose()
        {
            _disposals.Dispose();
        }
    }
}