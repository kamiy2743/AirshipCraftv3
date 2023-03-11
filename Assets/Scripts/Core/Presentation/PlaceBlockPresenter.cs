using System;
using ACv3.Domain;
using UniRx;
using ACv3.UnityView.Players;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Presentation
{
    public class PlaceBlockPresenter : IInitializable, IDisposable
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