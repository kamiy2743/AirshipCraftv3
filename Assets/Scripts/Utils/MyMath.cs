using UnityEngine;

namespace ACv3.Utils
{
    public static class MyMath
    {
        public static int Repeat(int x, int minInclusive, int maxInclusive)
        {
            return (int)Mathf.Repeat(x, maxInclusive - minInclusive + 1) + minInclusive;
        }
    }
}