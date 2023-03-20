using System;
using ACv3.Domain.Inventories;
using ACv3.Extensions;
using ACv3.Presentation.Inputs;
using ACv3.UseCase;
using UniRx;
using Zenject;

namespace ACv3.Presentation
{
    public class InventoryInputHandler : IInitializable, IDisposable
    {
        readonly IInputController inputController;
        readonly InventoryStateController inventoryStateController;
        readonly CompositeDisposable disposable = new();

        [Inject]
        InventoryInputHandler(IInputController inputController, InventoryStateController inventoryStateController)
        {
            this.inputController = inputController;
            this.inventoryStateController = inventoryStateController;
        }

        void IInitializable.Initialize()
        {
            ObservableExt.SmartAny(
                    inputController.OnOpenPlayerInventoryRequested(),
                    inputController.OnCloseInventoryRequested())
                .Subscribe(winType =>
                {
                    if (winType == ObservableExt.WinType.Left)
                    {
                        inventoryStateController.Open(InventoryId.PlayerInventory);
                        return;
                    }

                    if (winType == ObservableExt.WinType.Right)
                    {
                        inventoryStateController.InventoryClose();
                        return;
                    }

                    if (inventoryStateController.IsOpened)
                    {
                        inventoryStateController.InventoryClose();
                    }
                    else
                    {
                        inventoryStateController.Open(InventoryId.PlayerInventory);
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