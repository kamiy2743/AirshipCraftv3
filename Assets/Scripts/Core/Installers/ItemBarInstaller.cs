using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using Zenject;

class ItemBarInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ItemBarModel>().AsSingle();
        Container.Bind<ItemBarView>().AsSingle();
        Container.BindInterfacesTo<ItemBarPresenter>().AsSingle();
    }
}