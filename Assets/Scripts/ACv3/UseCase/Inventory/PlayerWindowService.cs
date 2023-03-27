using System;
using ACv3.Domain.Inventories;
using ACv3.Domain.Items;
using ACv3.Domain.Windows;
using UniRx;
using Zenject;

namespace ACv3.UseCase.Inventory
{
    public class PlayerWindowService
    {
        readonly PlayerWindow playerWindow;
        readonly IInventory playerInventory;
        
        [Inject]
        PlayerWindowService(WindowProvider windowProvider)
        {
            playerWindow = new PlayerWindow();
            playerInventory = playerWindow;
            windowProvider.AddWindow(playerWindow);

            playerInventory.SetSlot(new PlayerInventorySlotId(0, 0), new Slot(new StackItem(new Dirt(), new Amount(24))));
            playerInventory.SetSlot(new PlayerInventorySlotId(1, 4), new Slot(new StackItem(new Dirt(), new Amount(45))));
            playerInventory.SetSlot(new PlayerInventorySlotId(3, 6), new Slot(new StackItem(new Dirt(), new Amount(80))));
            playerInventory.SetSlot(new PlayerInventorySlotId(1, 5), new Slot(new StackItem(new Stone(), new Amount(24))));
            playerInventory.SetSlot(new PlayerInventorySlotId(2, 3), new Slot(new StackItem(new Stone(), new Amount(45))));
            playerInventory.SetSlot(new PlayerInventorySlotId(3, 2), new Slot(new StackItem(new Stone(), new Amount(80))));
        }

        public IObservable<(PlayerInventorySlotId slotId, Slot slot)> OnSlotUpdated =>
            playerInventory.OnSlotUpdated
                .Select(x => ((PlayerInventorySlotId)x.slotId, x.slot))
                .Publish()
                .RefCount();
    }
}