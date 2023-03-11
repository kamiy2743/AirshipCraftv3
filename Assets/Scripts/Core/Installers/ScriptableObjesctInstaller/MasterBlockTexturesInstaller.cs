using ACv3.MasterData;
using UnityEngine;
using Zenject;

namespace ACv3.Installers
{
    [CreateAssetMenu(fileName = "MasterBlockTexturesInstaller", menuName = "Installers/MasterBlockTexturesInstaller")]
    class MasterBlockTexturesInstaller : ScriptableObjectInstaller<MasterBlockTexturesInstaller>
    {
        [SerializeField] MasterBlockTextures instance;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MasterBlockTextures>().FromInstance(instance).AsSingle();
        }
    }
}