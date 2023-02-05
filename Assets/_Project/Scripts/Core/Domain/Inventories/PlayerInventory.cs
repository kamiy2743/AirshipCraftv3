using System;
using System.Collections.Generic;
using Domain.Items;

namespace Domain.Inventories
{
    internal class PlayerInventory : IInventory
    {
        private Dictionary<PlayerInventorySlotID, Slot> slots;

        internal const int LineCount = 4;
        internal const int RowCount = 9;

        internal PlayerInventory()
        {
            slots = new Dictionary<PlayerInventorySlotID, Slot>(LineCount * RowCount);
        }

        internal void SetItem(PlayerInventorySlotID slotID, IItem item, Amount amount)
        {
            slots[slotID].SetItem(item, amount);
        }
    }

    internal record PlayerInventorySlotID
    {
        private int line;
        private int row;

        internal PlayerInventorySlotID(int line, int row)
        {
            if (line < 0 || line >= PlayerInventory.LineCount) throw new ArgumentOutOfRangeException(line.ToString());
            if (row < 0 || row >= PlayerInventory.RowCount) throw new ArgumentOutOfRangeException(row.ToString());

            this.line = line;
            this.row = row;
        }
    }
}