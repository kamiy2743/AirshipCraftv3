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

        public static PlayerInventorySlotId Empty() => new(EmptyLine, EmptyRow);

        public override string ToString() => $"PlayerInventorySlotId: {line}, {row}";

        public GlobalInventorySlotId ToGlobalInventorySlotId() 
        {
            return new GlobalInventorySlotId(InventoryId.PlayerInventory, line * RowCount + row);
        }

        public static PlayerInventorySlotId FromGlobalInventorySlotId(GlobalInventorySlotId globalInventorySlotId)
        {
            var rawValue = globalInventorySlotId.RawValue;
            var line = rawValue / RowCount;
            var row = rawValue % RowCount;
            return new PlayerInventorySlotId(line, row);
        }
    }
}