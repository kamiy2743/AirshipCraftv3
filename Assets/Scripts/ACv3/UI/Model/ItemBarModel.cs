using System;
using ACv3.Domain.Inventories;
using UniRx;
using Unity.Mathematics;

namespace ACv3.UI.Model
{
    public class ItemBarModel
    {
        readonly ReactiveProperty<ItemBarSlotId> selectedSlotId = new(ItemBarSlotId.Default());
        public IObservable<ItemBarSlotId> SelectedSlotIdAsObservable => selectedSlotId;

        public void Scroll(ItemBarScrollDirection scrollDirection)
        {
            var value = selectedSlotId.Value.ToInt();
            if (scrollDirection == ItemBarScrollDirection.Left)
            {
                value--;
            }
            else
            {
                value++;
            }

            selectedSlotId.Value = new ItemBarSlotId(math.clamp(value, ItemBarSlotId.Min, ItemBarSlotId.Max));
        }
    }
}