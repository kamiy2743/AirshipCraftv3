using System;
using ACv3.Domain.Inventories;
using ACv3.Presentation.Inputs;
using ACv3.UseCase.Window;
using ACv3.UseCase.Inventory;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UI.Model
{
    public class WindowCanvasModel
    {
        readonly WindowService windowService;
        readonly InventoryService inventoryService;
        readonly IInputController inputController;

        [Inject]
        WindowCanvasModel(WindowService windowService, InventoryService inventoryService, IInputController inputController)
        {
            this.windowService = windowService;
            this.inventoryService = inventoryService;
            this.inputController = inputController;
        }

        public IObservable<Unit> OpenRequested() => windowService.HasOpenWindow.Where(x => x).AsUnitObservable();
        public IObservable<Unit> CloseRequested() => windowService.HasOpenWindow.Where(x => !x).AsUnitObservable();

        public IObservable<GrabbingInventoryItem> OnGrabItemStarted() => inventoryService.OnGrabItemStarted();
        public IObservable<Unit> OnGrabItemEnded() => inventoryService.OnGrabItemEnded();
        public IReadOnlyReactiveProperty<Vector2> PointerPosition() => inputController.PointerPosition();
    }
}