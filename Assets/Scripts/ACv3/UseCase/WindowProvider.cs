using System.Collections.Generic;
using System.Linq;
using ACv3.Domain.Inventories;
using ACv3.Domain.Windows;
using UnityEngine;

namespace ACv3.UseCase
{
    public class WindowProvider
    {
        readonly Dictionary<WindowId, IWindow> windows = new();
        readonly Dictionary<InventoryId, IInventory> inventories = new();

        public void AddWindow(IWindow window) => windows[window.WindowId] = window;
        public IWindow GetWindow(WindowId windowId) => windows[windowId];

        public Dictionary<InventoryId, IInventory> GetInventories() => windows.Values.OfType<IInventory>().ToDictionary(x => x.InventoryId);
    }
}