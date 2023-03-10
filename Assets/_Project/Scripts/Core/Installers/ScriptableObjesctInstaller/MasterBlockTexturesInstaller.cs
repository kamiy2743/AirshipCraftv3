using UnityEngine;
using Zenject;
using MasterData;

namespace Installers
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