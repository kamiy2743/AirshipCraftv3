using System;
using System.Collections.Generic;
using Domain.Items;

namespace Domain.Inventories
{
    class PlayerInventory : IInventory
    {
        readonly Dictionary<PlayerInventorySlotID, Slot> _slots;

        internal const int LineCount = 4;
        internal const int RowCount = 9;

        internal PlayerInventory()
        {
            _slots = new Dictionary<PlayerInventorySlotID, Slot>(LineCount * RowCount);
        }

        internal void SetItem(PlayerInventorySlotID slotID, IItem item, Amount amount)
        {
            _slots[slotID].SetItem(item, amount);
        }
    }

    record PlayerInventorySlotID
    {
        int _line;
        int _row;

        internal PlayerInventorySlotID(int line, int row)
        {
            if (line < 0 || line >= PlayerInventory.LineCount) throw new ArgumentOutOfRangeException(line.ToString());
            if (row < 0 || row >= PlayerInventory.RowCount) throw new ArgumentOutOfRangeException(row.ToString());

            _line = line;
            _row = row;
        }
    }
}