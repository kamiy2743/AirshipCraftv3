using System;
using ACv3.Domain.Inventories;
using ACv3.Utils;
using UniRx;
using UnityEngine;

namespace ACv3.UI.Model
{
    public class ItemBarModel
    {
        readonly ReactiveProperty<ItemBarSlotId> selectedSlotID = new(ItemBarSlotId.CreateMin());
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

            selectedSlotID.Value = new ItemBarSlotId(MyMath.Repeat(value, ItemBarSlotId.Min, ItemBarSlotId.Max));
        }
    }
}