using ACv3.Domain.Inventories;
using ACv3.UseCase;
using UniRx;
using Zenject;

namespace ACv3.UI.Model
{
    public class GrabbingInventoryItemModel
    {
        readonly GrabInventoryItemService grabInventoryItemService;

        [Inject]
        GrabbingInventoryItemModel(GrabInventoryItemService grabInventoryItemService)
        {
            this.grabInventoryItemService = grabInventoryItemService;
        }

        public IReadOnlyReactiveProperty<GrabbingInventoryItem> GetGrabbingItem() => grabInventoryItemService.GrabbingItem;
    }
}