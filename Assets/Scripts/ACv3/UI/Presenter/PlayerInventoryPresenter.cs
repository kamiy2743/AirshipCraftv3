using System;
using ACv3.Domain.Inventories;
using ACv3.Extensions;
using ACv3.UI.Model;
using ACv3.UI.View;
using ACv3.UnityView.Inputs;
using UniRx;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class PlayerInventoryPresenter : IInitializable, IDisposable
    {
        readonly PlayerInventoryView view;
        readonly PlayerInventoryModel model;
        readonly IInputController inputController;

        readonly CompositeDisposable disposable = new();

        [Inject]
        PlayerInventoryPresenter(PlayerInventoryView view, PlayerInventoryModel model, IInputController inputController)
        {
            this.view = view;
            this.model = model;
            this.inputController = inputController;
        }
        
        void IInitializable.Initialize()
        {
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
            
            ObservableExt.SmartAny(
                    inputController.OnOpenPlayerInventoryRequested(),
                    inputController.OnCloseInventoryRequested())
                .Subscribe(winType  => 
                {
                    if (winType == ObservableExt.WinType.Left)
                    {
                        model.Open();
                        return;
                    }
                    
                    if (winType == ObservableExt.WinType.Right)
                    {
                        model.Close();
                        return;
                    }
                    
                    if (model.IsOpened.Value)
                    {
                        model.Close();
                    }
                    else
                    {
                        model.Open();
                    }
                });

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
                    view.SetItem(value.slotId, null, value.slot.Amount);
                })
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}