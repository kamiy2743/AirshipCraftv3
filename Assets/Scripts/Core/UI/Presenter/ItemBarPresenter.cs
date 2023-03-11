using System;
using ACv3.UI.Model;
using ACv3.UI.View;
using ACv3.UnityView.Inputs;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class ItemBarPresenter : IInitializable, IDisposable
    {
        readonly ItemBarModel model;
        readonly ItemBarView view;
        readonly IInputProvider inputProvider;
        
        readonly CompositeDisposable disposable = new();

        [Inject]
        ItemBarPresenter(ItemBarModel model, ItemBarView view, IInputProvider inputProvider)
        {
            this.model = model;
            this.view = view;
            this.inputProvider = inputProvider;
        }
        
        void IInitializable.Initialize()
        {
            inputProvider.OnItemBarScroll
                .Subscribe(model.Scroll)
                .AddTo(disposable);
            
            model.SelectedSlotIDAsObservable
                .Subscribe(view.SetSelectedSlot)
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}