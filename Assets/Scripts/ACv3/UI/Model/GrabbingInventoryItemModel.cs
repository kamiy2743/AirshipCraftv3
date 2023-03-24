using ACv3.Domain.Inventories;
using ACv3.Presentation.Inputs;
using ACv3.UseCase;
using UniRx;
using UnityEngine;
using Zenject;

namespace ACv3.UI.Model
{
    public class GrabbingInventoryItemModel
    {
        readonly GrabInventoryItemService grabInventoryItemService;
        readonly IInputController inputController;

        [Inject]
        GrabbingInventoryItemModel(GrabInventoryItemService grabInventoryItemService, IInputController inputController)
        {
            this.grabInventoryItemService = grabInventoryItemService;
            this.inputController = inputController;
        }

        public IReadOnlyReactiveProperty<GrabbingInventoryItem> GrabbingItem() => grabInventoryItemService.GrabbingItem;
        public IReadOnlyReactiveProperty<Vector2> PointerPosition() => inputController.PointerPosition();
    }
}