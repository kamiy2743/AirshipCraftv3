using UnityEngine;
using Zenject;
using MasterData;

namespace Installers
{
    [CreateAssetMenu(fileName = "MasterBlockTexturesInstaller", menuName = "Installers/MasterBlockTexturesInstaller")]
    internal class MasterBlockTexturesInstaller : ScriptableObjectInstaller<MasterBlockTexturesInstaller>
    {
        [SerializeField] private MasterBlockTextures instance;

        public override void InstallBindings()
        {
            Container.BindInstance<MasterBlockTextures>(instance).AsSingle();
        }
    }
}