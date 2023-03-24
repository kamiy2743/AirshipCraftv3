using System;
using ACv3.Domain.Inventories;
using ACv3.UI.Model;
using ACv3.UI.View;
using UniRx;
using UnityEngine;
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
            model.GrabbingItem()
                .Subscribe(grabbingItem =>
                {
                    if (!grabbingItem.IsEmpty)
                    {
                        view.SetActive(true);
                        view.SetItem(null, grabbingItem.Item.Amount, grabbingItem.Item.ItemId.RawString());
                    }
                    else
                    {
                        view.SetActive(false);
                    }
                })
                .AddTo(disposable);

            model.PointerPosition()
                .Subscribe(view.SetPosition)
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}