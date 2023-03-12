using Cysharp.Threading.Tasks;
using ACv3.UseCase;
using Zenject;

namespace ACv3.Presentation
{
    public class EnterWorldModel
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