using System;
using System.Linq;
using ACv3.Domain.Windows;
using UniRx;
using Zenject;

namespace ACv3.UseCase.Window
{
    public class WindowService
    {
        readonly WindowProvider windowProvider;
        readonly ReactiveDictionary<WindowId, IWindow> openWindows = new();
        readonly ReactiveProperty<bool> hasOpenWindow = new(false);

        public IReadOnlyReactiveProperty<bool> HasOpenWindow => hasOpenWindow.DistinctUntilChanged().ToReadOnlyReactiveProperty();

        public IObservable<Unit> OpenRequested(WindowId windowId) => 
            openWindows.ObserveAdd()
                .Where(e => e.Value.WindowId == windowId)
                .AsUnitObservable().Publish().RefCount();
        
        public IObservable<Unit> CloseRequested(WindowId windowId) => 
            openWindows.ObserveRemove()
                .Where(e => e.Value.WindowId == windowId)
                .AsUnitObservable().Publish().RefCount();

        public bool IsOpened(WindowId windowId) => openWindows.ContainsKey(windowId);

        [Inject]
        WindowService(WindowProvider windowProvider)
        {
            this.windowProvider = windowProvider;
        }

        public void Open(WindowId windowId)
        {
            if (openWindows.ContainsKey(windowId)) return;
            openWindows.Add(windowId, windowProvider.GetWindow(windowId));
            hasOpenWindow.Value = true;
        }

        void Close(WindowId windowId)
        {
            if (!openWindows.ContainsKey(windowId)) return;
            openWindows.Remove(windowId);
            
            if (openWindows.Count == 0)
            {
                hasOpenWindow.Value = false;
            }
        }

        public void CloseAll()
        {
            var keys = openWindows.Keys.ToList();
            foreach (var windowId in keys)
            {
                Close(windowId);
            }
        }
    }
}