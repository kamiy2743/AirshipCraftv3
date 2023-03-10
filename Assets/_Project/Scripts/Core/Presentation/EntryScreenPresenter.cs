using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Presentation
{
    internal class EntryScreenPresenter : MonoBehaviour
    {
        [Inject] private EnterWorldModel enterWorldModel;

        [SerializeField] private Button enterWorldButton;

        private void Start()
        {
            enterWorldButton
                .BindToOnClick(_ => enterWorldModel.EnterWorldAsync().ToObservable().AsUnitObservable())
                .AddTo(this);
        }
    }
}
