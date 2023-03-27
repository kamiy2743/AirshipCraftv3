using System;
using ACv3.Domain.Inventories;
using ACv3.Domain.Windows;
using ACv3.UseCase.Inventory;
using ACv3.UseCase.Window;
using UniRx;
using Zenject;

namespace ACv3.UI.Model
{
    public class PlayerWindowModel
    {
        readonly PlayerWindowService playerWindowService;
        readonly WindowService windowService;
        readonly InventoryService inventoryService;

        [Inject]
        PlayerWindowModel(PlayerWindowService playerWindowService, WindowService windowService, InventoryService inventoryService)
        {
            this.playerWindowService = playerWindowService;
            this.windowService = windowService;
            this.inventoryService = inventoryService;
        }

        public IObservable<Unit> OpenRequested() => windowService.OpenRequested(WindowId.PlayerWindow);
        public IObservable<Unit> CloseRequested() => windowService.CloseRequested(WindowId.PlayerWindow);

        public IReadOnlyReactiveProperty<PlayerInventorySlotId> SelectedSlotId() =>
            inventoryService.SelectedSlotId
                .Select(slotId => (PlayerInventorySlotId)slotId)
                .ToReadOnlyReactiveProperty();

        public void SetSelectedSlotId(PlayerInventorySlotId slotId) => inventoryService.SetSelectedSlotId(slotId);
        
        public void GrabStartOrEndRequest(PlayerInventorySlotId slotId) => inventoryService.GrabStartOrEndRequest(slotId);

        public IObservable<(PlayerInventorySlotId slotId, Slot slot)> OnSlotUpdated => playerWindowService.OnSlotUpdated;
    }
}