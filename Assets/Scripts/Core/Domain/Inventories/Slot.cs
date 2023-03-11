using System;
using ACv3.Domain.Items;

namespace ACv3.Domain.Inventories
{
    class Slot
    {
        IItem item;
        Amount amount;

        internal void SetItem(IItem item, Amount amount)
        {
            if (amount > item.GetMaxAmount())
            {
                throw new Exception($"max amount is {item.GetMaxAmount()}, but given {amount}");
            }

            this.item = item;
            this.amount = amount;
        }
    }
}