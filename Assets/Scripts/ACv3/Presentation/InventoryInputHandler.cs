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
        readonly InventoryService inventoryService;
        readonly CompositeDisposable disposable = new();

        [Inject]
        InventoryInputHandler(IInputController inputController, InventoryService inventoryService)
        {
            this.inputController = inputController;
            this.inventoryService = inventoryService;
        }

        void IInitializable.Initialize()
        {
            ObservableExt.SmartAny(
                    inputController.OnOpenPlayerInventoryRequested(),
                    inputController.OnCloseInventoryRequested())
                .Subscribe(winType =>
                {
                    UnityEngine.Debug.Log(winType);
                    if (winType == ObservableExt.WinType.Left)
                    {
                        inventoryService.Open(InventoryId.PlayerInventoryId);
                        return;
                    }

                    if (winType == ObservableExt.WinType.Right)
                    {
                        inventoryService.InventoryClose();
                        return;
                    }

                    if (inventoryService.IsOpened)
                    {
                        inventoryService.InventoryClose();
                    }
                    else
                    {
                        inventoryService.Open(InventoryId.PlayerInventoryId);
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