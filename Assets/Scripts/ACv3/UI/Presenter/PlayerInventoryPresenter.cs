using System;
using ACv3.Domain.Inventories;
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
            model.Initialize();
            view.Initialize();

            view.OnCursorSlot
                .Subscribe(slotId =>
                {
                    model.SetIsSelected(true);
                    model.SetSelectedSlotId(slotId);
                    model.SetSlot(slotId, Slot.Empty());
                })
                .AddTo(disposable);

            view.OnUnCursorSlot
                .Subscribe(_ => model.SetIsSelected(false))
                .AddTo(disposable);

            view.OnClickSlot
                .Subscribe(slotId => model.InvokeSlotClickedEvent(slotId))
                .AddTo(disposable);

            model.IsSelected
                .CombineLatest(model.SelectedSlotId, (isSelected, slotId) => (isSelected, slotId))
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

            model.IsOpened
                .Subscribe(isOpened =>
                {
                    if (isOpened)
                    {
                        view.Show();
                    }
                    else
                    {
                        view.Hide();
                    }
                })
                .AddTo(disposable);

            model.OnUpdateSlot
                .Subscribe(value =>
                {
                    view.SetItem(value.slotId, null, value.slot.Amount, value.slot.Item.ItemId.RawString());
                })
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}