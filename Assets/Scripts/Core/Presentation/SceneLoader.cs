using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ACv3.Presentation
{
    public class SceneLoader
    {
        internal async UniTask LoadSceneAsync(SceneName sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName.ToStringName());
        }
    }
}