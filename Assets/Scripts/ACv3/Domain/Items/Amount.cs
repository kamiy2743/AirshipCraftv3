using System;

namespace ACv3.Domain.Items
{
    public record Amount : IComparable<Amount>
    {
        readonly int value;

        public Amount(int value)
        {
            if (value < 0) throw new ArgumentException();
            this.value = value;
        }

        public int CompareTo(Amount other)
        {
            if (value < other.value) return -1;
            if (value > other.value) return 1;
            return 0;
        }

        public string ToStringValue()
        {
            return value.ToString();
        }

        public static bool operator >(Amount a, Amount b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(Amount a, Amount b)
        {
            return a.CompareTo(b) < 0;
        }
        
        public static bool operator >=(Amount a, Amount b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(Amount a, Amount b)
        {
            return a.CompareTo(b) <= 0;
        }
    }
}