using MasterData;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshesInstaller", menuName = "Installers/MasterBlockShapeMeshesInstaller")]
    class MasterBlockShapeMeshesInstaller : ScriptableObjectInstaller<MasterBlockShapeMeshesInstaller>
    {
        [SerializeField] MasterBlockShapeMeshes instance;

        public override void InstallBindings()
        {
            Container.BindInstance(instance).AsSingle();
        }
    }
}