using System;
using ACv3.Domain.Inventories;
using ACv3.UI.Model;
using ACv3.UI.View;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class PlayerWindowPresenter : IInitializable, IDisposable
    {
        readonly PlayerWindowView view;
        readonly PlayerWindowModel model;
        readonly CompositeDisposable disposable = new();

        [Inject]
        PlayerWindowPresenter(PlayerWindowView view, PlayerWindowModel model)
        {
            this.view = view;
            this.model = model;
        }
        
        void IInitializable.Initialize()
        {
            view.Initialize();
            
            view.OnCursorSlot
                .Subscribe(model.SetSelectedSlotId)
                .AddTo(disposable);
                
            view.OnUnCursorSlot
                .Subscribe(_ => model.SetSelectedSlotId(PlayerInventorySlotId.Empty))
                .AddTo(disposable);
                
            view.OnClickSlot
                .Subscribe(model.GrabStartOrEndRequest)
                .AddTo(disposable);

            model.OpenRequested()
                .Subscribe(_ => view.Open())
                .AddTo(disposable);
            
            model.CloseRequested()
                .Subscribe(_ => view.Close())
                .AddTo(disposable);

            model.SelectedSlotId()
                .Subscribe(slotId =>
                {
                    if (slotId.IsEmpty)
                    {
                        view.DeselectSlot();
                    }
                    else
                    {
                        view.SetSelectedSlot(slotId);
                    }
                })
                .AddTo(disposable);

            model.OnSlotUpdated
                .Subscribe(value =>
                {
                    view.SetItem(value.slotId, null, value.slot.Amount, value.slot.ItemId.DisplayString());
                })
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}