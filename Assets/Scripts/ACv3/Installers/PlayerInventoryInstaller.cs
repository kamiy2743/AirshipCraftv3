using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using UnityEngine;
using Zenject;

namespace ACv3.Installers
{
    class PlayerInventoryInstaller : MonoInstaller
    {
        [SerializeField] PlayerInventoryView itemBarView;

        public override void InstallBindings()
        {
            Container.Bind<PlayerInventoryModel>().AsSingle();
            Container.BindInstance(itemBarView).AsSingle();
            Container.BindInterfacesTo<PlayerInventoryPresenter>().AsSingle();
        }
    }
}