using System;
using ACv3.UI.Model;
using ACv3.UI.View;
using UniRx;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class PlayerInventoryPresenter : IInitializable, IDisposable
    {
        readonly PlayerInventoryView view;
        readonly PlayerInventoryModel model;

        readonly CompositeDisposable disposable = new();

        [Inject]
        PlayerInventoryPresenter(PlayerInventoryView view, PlayerInventoryModel model)
        {
            this.view = view;
            this.model = model;
        }
        
        void IInitializable.Initialize()
        {
            view.Initialize();

            view.OnCursorSlot
                .Subscribe(slotId =>
                {
                    model.SetIsSelected(true);
                    model.SetSelectedSlotId(slotId);
                })
                .AddTo(disposable);

            view.OnUnCursorSlot
                .Subscribe(_ => model.SetIsSelected(false))
                .AddTo(disposable);

            model.IsSelectedAsObservable
                .CombineLatest(model.SelectedSlotIdAsObservable, (isSelected, slotId) => (isSelected, slotId))
                .Subscribe(value =>
                {
                    if (value.isSelected)
                    {
                        view.SetSelectedSlot(value.slotId);
                    }
                    else
                    {
                        view.DeselectSlot();
                    }
                })
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}