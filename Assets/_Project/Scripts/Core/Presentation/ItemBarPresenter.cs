using System;
using Domain.Players;
using UnityView.Players;
using Zenject;
using UniRx;

namespace Presentation
{
    class ItemBarPresenter : IInitializable, IDisposable
    {
        readonly ItemBar _itemBar;
        readonly ItemBarView _itemBarView;

        readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        ItemBarPresenter(ItemBar itemBar, ItemBarView itemBarView)
        {
            _itemBar = itemBar;
            _itemBarView = itemBarView;
        }

        public void Initialize()
        {
            _itemBar
                .SelectedSlotAsObservable
                .Subscribe(selectedSlotID => _itemBarView.SetSelectedSlot(selectedSlotID))
                .AddTo(_compositeDisposable);

            _itemBarView
                .OnScrolled
                .Subscribe(scrollType => _itemBar.Scroll(scrollType))
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}