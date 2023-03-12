using System;
using ACv3.Domain.Inventories;
using UniRx;
using Unity.Mathematics;

namespace ACv3.UI.Model
{
    public class ItemBarModel
    {
        readonly ReactiveProperty<ItemBarSlotId> selectedSlotID = new(ItemBarSlotId.Default());
        public IObservable<ItemBarSlotId> SelectedSlotIDAsObservable => selectedSlotID;

        public void Scroll(ItemBarScrollDirection scrollDirection)
        {
            var value = selectedSlotID.Value.ToInt();
            if (scrollDirection == ItemBarScrollDirection.Left)
            {
                value--;
            }
            else
            {
                value++;
            }

            selectedSlotID.Value = new ItemBarSlotId(math.clamp(value, ItemBarSlotId.Min, ItemBarSlotId.Max));
        }
    }
}