using Cysharp.Threading.Tasks;
using UseCase;
using Zenject;

namespace Presentation
{
    class EnterWorldModel
    {
        [Inject] SceneLoader sceneLoader;
        [Inject] EnterWorldUseCase enterWorldUseCase;

        internal async UniTask EnterWorldAsync()
        {
            await sceneLoader.LoadSceneAsync(SceneName.World);
            enterWorldUseCase.EnterWorld();
        }
    }
}