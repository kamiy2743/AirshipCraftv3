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
        
        public static Amount Empty() => new(0);

        public string RawString() => value.ToString();
        public override string ToString() => $"Amount: {value}";

        public static Amount operator +(Amount a, Amount b) => new(a.value + b.value);
        public static Amount operator -(Amount a, Amount b) => new(a.value - b.value);

        public int CompareTo(Amount other) => value.CompareTo(other.value);
        public static bool operator >(Amount a, Amount b) => a.CompareTo(b) > 0;
        public static bool operator <(Amount a, Amount b) => a.CompareTo(b) < 0;
        public static bool operator >=(Amount a, Amount b) => a.CompareTo(b) >= 0;
        public static bool operator <=(Amount a, Amount b) => a.CompareTo(b) <= 0;
    }
}