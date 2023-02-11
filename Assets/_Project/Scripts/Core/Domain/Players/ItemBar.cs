using System;
using System.Collections.Generic;
using Domain.Inventories;
using UniRx;

namespace Domain.Players
{
    public class ItemBar
    {
        readonly Dictionary<ItemBarSlotID, Slot> _slots;

        readonly BehaviorSubject<ItemBarSlotID> _selectedSlot;
        public IObservable<ItemBarSlotID> SelectedSlotAsObservable => _selectedSlot;

        internal const int SlotCount = 10;

        ItemBar()
        {
            _selectedSlot = new BehaviorSubject<ItemBarSlotID>(new ItemBarSlotID(0));
            _slots = new Dictionary<ItemBarSlotID, Slot>(SlotCount);
        }

        public void Scroll(ItemBarScrollType scrollType)
        {
            var nextSlotID = _selectedSlot.Value.ToInt();
            
            switch (scrollType)
            {
                case ItemBarScrollType.Right :
                    nextSlotID++;
                    break;
                case ItemBarScrollType.Left:
                    nextSlotID--;
                    break;
                case ItemBarScrollType.None :
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scrollType), scrollType, null);
            }
            
            if (nextSlotID < 0 || nextSlotID >= SlotCount) return;
            
            _selectedSlot.OnNext(new ItemBarSlotID(nextSlotID));
        }
    }
}