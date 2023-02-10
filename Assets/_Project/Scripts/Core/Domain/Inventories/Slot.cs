using System;
using Domain.Items;

namespace Domain.Inventories
{
    class Slot
    {
        IItem _item;
        Amount _amount;

        internal void SetItem(IItem item, Amount amount)
        {
            if (amount > item.GetMaxAmount())
            {
                throw new Exception($"max amount is {item.GetMaxAmount()}, but given {amount}");
            }

            _item = item;
            _amount = amount;
        }
    }
}