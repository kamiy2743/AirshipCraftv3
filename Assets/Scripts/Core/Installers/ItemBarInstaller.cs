using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using UnityEngine;
using Zenject;

class ItemBarInstaller : MonoInstaller
{
    [SerializeField] ItemBarView itemBarView;
    
    public override void InstallBindings()
    {
        Container.Bind<ItemBarModel>().AsSingle();
        Container.BindInstance(itemBarView).AsSingle();
        Container.BindInterfacesTo<ItemBarPresenter>().AsSingle();
    }
}