using UnityEngine;
using Zenject;
using MasterData;

namespace Installers
{
    [CreateAssetMenu(fileName = "MasterBlockShapeMeshesInstaller", menuName = "Installers/MasterBlockShapeMeshesInstaller")]
    internal class MasterBlockShapeMeshesInstaller : ScriptableObjectInstaller<MasterBlockShapeMeshesInstaller>
    {
        [SerializeField] private MasterBlockShapeMeshes instance;

        public override void InstallBindings()
        {
            Container.BindInstance<MasterBlockShapeMeshes>(instance).AsSingle();
        }
    }
}