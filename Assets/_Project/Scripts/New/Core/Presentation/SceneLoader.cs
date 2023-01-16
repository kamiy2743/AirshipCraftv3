using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Presentation
{
    internal class SceneLoader
    {
        internal async UniTask LoadSceneAsync(SceneName sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName.ToStringName());
        }
    }
}