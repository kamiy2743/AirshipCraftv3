using Zenject;
using UseCase;
using Cysharp.Threading.Tasks;

namespace Presentation
{
    internal class EnterWorldModel
    {
        [Inject] private SceneLoader sceneLoader;
        [Inject] private EnterWorldUseCase enterWorldUseCase;

        internal async UniTask EnterWorldAsync()
        {
            await sceneLoader.LoadSceneAsync(SceneName.World);
            enterWorldUseCase.EnterWorld();
        }
    }
}