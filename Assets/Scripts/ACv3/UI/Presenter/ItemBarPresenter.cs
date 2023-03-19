using System;
using ACv3.UI.Model;
using ACv3.UI.View;
using ACv3.Presentation.Inputs;
using UniRx;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class ItemBarPresenter : IInitializable, IDisposable
    {
        readonly ItemBarModel model;
        readonly ItemBarView view;
        readonly IInputController inputController;
        
        readonly CompositeDisposable disposable = new();

        [Inject]
        ItemBarPresenter(ItemBarModel model, ItemBarView view, IInputController inputController)
        {
            this.model = model;
            this.view = view;
            this.inputController = inputController;
        }
        
        void IInitializable.Initialize()
        {
            view.Initialize();
            
            inputController.OnItemBarScroll()
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