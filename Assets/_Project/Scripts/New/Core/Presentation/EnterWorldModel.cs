using Zenject;
using UseCase;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UniRx;

namespace Presentation
{
    internal class EnterWorldModel
    {
        [Inject] private EnterWorldUseCase enterWorldUseCase;

        internal async UniTask EnterWorldAsync()
        {
            // TODO マジックString
            await SceneManager.LoadSceneAsync("WorldScene");
            enterWorldUseCase.EnterWorld();
        }
    }
}