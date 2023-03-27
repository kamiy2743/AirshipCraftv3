using ACv3.Presentation;
using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using ACv3.UseCase;
using ACv3.UseCase.Window;
using ACv3.UseCase.Inventory;
using UnityEngine;
using Zenject;

namespace ACv3.Installers
{
    class WindowCanvasInstaller : MonoInstaller
    {
        [SerializeField] WindowCanvasView windowCanvasView;
    
        public override void InstallBindings()
        {
            Container.Bind<WindowProvider>().AsSingle();
            Container.BindInterfacesTo<PlayerWindowInputHandler>().AsSingle();

            Container.Bind<WindowService>().AsSingle();
            Container.Bind<InventoryService>().AsSingle();
            
            Container.Bind<WindowCanvasModel>().AsSingle();
            Container.BindInstance(windowCanvasView).AsSingle();
            Container.BindInterfacesTo<WindowCanvasPresenter>().AsSingle();
        }
    }
}