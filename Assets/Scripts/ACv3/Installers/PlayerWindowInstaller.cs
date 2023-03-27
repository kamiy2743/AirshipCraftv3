using ACv3.UI.Model;
using ACv3.UI.Presenter;
using ACv3.UI.View;
using ACv3.UseCase.Inventory;
using UnityEngine;
using Zenject;

namespace ACv3.Installers
{
    class PlayerWindowInstaller : MonoInstaller
    {
        [SerializeField] PlayerWindowView playerWindowView;

        public override void InstallBindings()
        {
            Container.Bind<PlayerWindowService>().AsSingle();
            Container.Bind<PlayerWindowModel>().AsSingle();
            Container.BindInstance(playerWindowView).AsSingle();
            Container.BindInterfacesTo<PlayerWindowPresenter>().AsSingle();
        }
    }
}