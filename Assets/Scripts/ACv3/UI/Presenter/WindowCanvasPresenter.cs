using System;
using ACv3.UI.Model;
using ACv3.UI.View;
using UniRx;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class WindowCanvasPresenter : IInitializable, IDisposable
    {
        readonly WindowCanvasView view;
        readonly WindowCanvasModel model;
        readonly CompositeDisposable disposable = new();

        [Inject]
        WindowCanvasPresenter(WindowCanvasView view, WindowCanvasModel model)
        {
            this.view = view;
            this.model = model;
        }

        void IInitializable.Initialize()
        {
            model.OpenRequested()
                .Subscribe(_ => view.Open())
                .AddTo(disposable);
            
            model.CloseRequested()
                .Subscribe(_ => view.Close())
                .AddTo(disposable);

            model.OnGrabItemStarted()
                .Subscribe(grabbingItem =>
                {
                    view.SetGrabbingItemActive(true);
                    view.SetGrabbingItem(null, grabbingItem.Item.Amount, grabbingItem.Item.ItemId.DisplayString());
                })
                .AddTo(disposable);

            model.OnGrabItemEnded()
                .Subscribe(_ => view.SetGrabbingItemActive(false))
                .AddTo(disposable);

            model.PointerPosition()
                .Subscribe(view.SetGrabbingItemPosition)
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}