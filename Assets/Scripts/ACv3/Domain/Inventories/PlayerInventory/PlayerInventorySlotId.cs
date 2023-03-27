using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record PlayerInventorySlotId
    {
        readonly int line;
        readonly int row;

        const int LineMin = 0;
        const int LineMax = 3;
        public const int LineCount = 4;
        
        const int RowMin = 0;
        const int RowMax = 8;
        public const int RowCount = 9;

        const int EmptyLine = int.MinValue;
        const int EmptyRow = int.MinValue;

        public PlayerInventorySlotId(int line, int row)
        {
            Assert.IsTrue((line >= LineMin && line <= LineMax) || line == EmptyLine);
            Assert.IsTrue((row >= RowMin && row <= RowMax) || row == EmptyRow);
            
            this.line = line;
            this.row = row;
        }

        public static PlayerInventorySlotId Empty => new(EmptyLine, EmptyRow);
        public bool IsEmpty => this == Empty;

        public override string ToString() => $"PlayerInventorySlotId: {line}, {row}";

        public static implicit operator GlobalInventorySlotId(PlayerInventorySlotId slotId)
        {
            if (slotId.IsEmpty) return GlobalInventorySlotId.Empty;
            return new GlobalInventorySlotId(InventoryId.PlayerInventory, slotId.line * RowCount + slotId.row);
        }

        public static implicit operator PlayerInventorySlotId(GlobalInventorySlotId slotId)
        {
            if (slotId.IsEmpty) return Empty;
            
            var rawValue = slotId.RawValue;
            var line = rawValue / RowCount;
            var row = rawValue % RowCount;
            return new PlayerInventorySlotId(line, row);
        }
    }
}