using System;
using ACv3.UI.Model;
using ACv3.UI.View;
using UniRx;
using Zenject;

namespace ACv3.UI.Presenter
{
    public class GrabbingInventoryItemPresenter : IInitializable, IDisposable
    {
        readonly GrabbingInventoryItemView view;
        readonly GrabbingInventoryItemModel model;

        readonly CompositeDisposable disposable = new();

        [Inject]
        GrabbingInventoryItemPresenter(GrabbingInventoryItemView view, GrabbingInventoryItemModel model)
        {
            this.view = view;
            this.model = model;
        }

        void IInitializable.Initialize()
        {
            model.GetGrabbingItem()
                .Subscribe(grabbingItem => view.SetItem(null, grabbingItem.Item.Amount, grabbingItem.Item.ItemId.RawString()))
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}