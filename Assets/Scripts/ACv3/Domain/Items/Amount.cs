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

        public static Amount Empty() => new Amount(0);

        public int CompareTo(Amount other)
        {
            if (value < other.value) return -1;
            if (value > other.value) return 1;
            return 0;
        }

        public string RawString() => value.ToString();
        public override string ToString() => $"Amount: {value}";

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