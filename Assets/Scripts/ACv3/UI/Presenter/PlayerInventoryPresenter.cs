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
            view.OnCursorSlot
                .Subscribe(slotId => model.SetSelectedSlotId(slotId))
                .AddTo(disposable);

            view.OnUnCursorSlot
                .Subscribe(_ => model.SetSelectedSlotId(PlayerInventorySlotId.Empty))
                .AddTo(disposable);

            view.OnClickSlot
                .Subscribe(slotId => model.InvokeSlotClickedEvent(slotId))
                .AddTo(disposable);

            model.SelectedSlotId
                .Subscribe(slotId =>
                {
                    if (!slotId.IsEmpty)
                    {
                        view.SetSelectedSlot(slotId);
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
                    view.SetItem(
                        value.slotId, 
                        null, 
                        value.slot.Item.Amount, 
                        value.slot.Item.ItemId.RawString());
                })
                .AddTo(disposable);
            
            view.Initialize();
            model.Initialize();
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}