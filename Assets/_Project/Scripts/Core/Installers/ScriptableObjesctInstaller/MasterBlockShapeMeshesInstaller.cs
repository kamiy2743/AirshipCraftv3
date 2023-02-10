using UnityEngine;
using Zenject;
using MasterData;

namespace Installers
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshesInstaller", menuName = "Installers/MasterBlockShapeMeshesInstaller")]
    class MasterBlockShapeMeshesInstaller : ScriptableObjectInstaller<MasterBlockShapeMeshesInstaller>
    {
        [SerializeField] MasterBlockShapeMeshes instance;

        public override void InstallBindings()
        {
            Container.BindInstance<MasterBlockShapeMeshes>(instance).AsSingle();
        }
    }
}