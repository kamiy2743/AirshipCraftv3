using ACv3.Domain.Inventories;
using UnityEngine;

namespace ACv3.UI.View
{
    public class ItemBarView
    {
        public void SetSelectedSlot(ItemBarSlotId slotId)
        {
            Debug.Log("view: "+slotId);
        }
    }
}