using System;

namespace Domain.Players
{
    public record ItemBarSlotID
    {
        readonly int _id;

        internal ItemBarSlotID(int id)
        {
            if (id < 0 || id >= ItemBar.SlotCount)
            {
                throw new ArgumentException(id.ToString());
            }
            
            _id = id;
        }

        public int ToInt() => _id;
    }
}