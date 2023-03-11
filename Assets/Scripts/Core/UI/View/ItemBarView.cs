using System.Collections.Generic;
using ACv3.Domain.Inventories;
using UnityEngine;

namespace ACv3.UI.View
{
    public class ItemBarView : MonoBehaviour
    {
        [SerializeField] SlotView[] _slotViews;

        readonly Dictionary<ItemBarSlotId, SlotView> slotViews = new();

        public void Initialize()
        {
            for (int i = 0; i <= ItemBarSlotId.Max; i++)
            {
                slotViews[new ItemBarSlotId(i)] = _slotViews[i];
            }
        }

        public void SetSelectedSlot(ItemBarSlotId slotId)
        {
            foreach (var id in slotViews.Keys)
            {
                slotViews[id].SetSelected(id == slotId);
            }
        }
    }
}