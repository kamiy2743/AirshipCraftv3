using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation
{
    class EntryScreenPresenter : MonoBehaviour
    {
        [Inject] EnterWorldModel enterWorldModel;

        [SerializeField] Button enterWorldButton;

        void Start()
        {
            enterWorldButton
                .BindToOnClick(_ => enterWorldModel.EnterWorldAsync().ToObservable().AsUnitObservable())
                .AddTo(this);
        }
    }
}
