using System;
using Domain.Items;

namespace Domain.Inventories
{
    internal class Slot
    {
        private IItem item;
        private Amount amount;

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