using Zenject;
using UseCase;
using Cysharp.Threading.Tasks;

namespace Presentation
{
    class EnterWorldModel
    {
        [Inject] SceneLoader _sceneLoader;
        [Inject] EnterWorldUseCase _enterWorldUseCase;

        internal async UniTask EnterWorldAsync()
        {
            await _sceneLoader.LoadSceneAsync(SceneName.World);
            _enterWorldUseCase.EnterWorld();
        }
    }
}