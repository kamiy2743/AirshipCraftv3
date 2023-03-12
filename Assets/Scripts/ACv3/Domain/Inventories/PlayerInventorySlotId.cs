using UnityEngine.Assertions;

namespace ACv3.Domain.Inventories
{
    public record PlayerInventorySlotId
    {
        readonly int line;
        readonly int row;

        const int LineMin = 0;
        const int LineMax = 3;
        
        const int RowMin = 0;
        const int RowMax = 8;
        public const int RowCount = 9;

        public PlayerInventorySlotId(int line, int row)
        {
            Assert.IsTrue(line >= LineMin && line <= LineMax);
            Assert.IsTrue(row >= RowMin && row <= RowMax);
            
            this.line = line;
            this.row = row;
        }

        public static PlayerInventorySlotId Default()
        {
            return new PlayerInventorySlotId(LineMin, RowMin);
        }

        public override string ToString()
        {
            return $"PlayerInventorySlotId: {line}, {row}";
        }
    }
}