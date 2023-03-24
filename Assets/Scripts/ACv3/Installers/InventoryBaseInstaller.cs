using ACv3.Presentation;
using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using ACv3.UseCase;
using UnityEngine;
using Zenject;

namespace ACv3.Installers
{
    class InventoryBaseInstaller : MonoInstaller
    {
        [SerializeField] GrabbingInventoryItemView grabbingInventoryItemView;
    
        public override void InstallBindings()
        {
            Container.Bind<InventoryBroker>().AsSingle();
            Container.Bind<InventoryStateController>().AsSingle();
            Container.BindInterfacesTo<InventoryInputHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GrabInventoryItemService>().AsSingle();
            Container.Bind<GrabbingInventoryItemModel>().AsSingle();
            Container.BindInstance(grabbingInventoryItemView).AsSingle();
            Container.BindInterfacesTo<GrabbingInventoryItemPresenter>().AsSingle();
        }
    }
}