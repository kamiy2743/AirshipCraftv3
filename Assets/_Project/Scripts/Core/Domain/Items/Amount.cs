using System;

namespace Domain.Items
{
    record Amount : IComparable<Amount>
    {
        readonly int _value;

        internal Amount(int value)
        {
            if (value < 0) throw new ArgumentException();
            _value = value;
        }

        public int CompareTo(Amount other)
        {
            if (_value < other._value) return -1;
            if (_value > other._value) return 1;
            return 0;
        }

        public static bool operator >(Amount a, Amount b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(Amount a, Amount b)
        {
            return a.CompareTo(b) < 0;
        }
    }
}