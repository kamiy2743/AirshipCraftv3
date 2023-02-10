using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Presentation
{
    class EntryScreenPresenter : MonoBehaviour
    {
        [Inject] EnterWorldModel _enterWorldModel;

        [SerializeField] Button enterWorldButton;

        void Start()
        {
            enterWorldButton
                .BindToOnClick(_ => _enterWorldModel.EnterWorldAsync().ToObservable().AsUnitObservable())
                .AddTo(this);
        }
    }
}
