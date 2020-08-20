using System;
using System.Collections.Generic;


namespace BobGreenhands.Utils
{
    public static class MathUtils
    {
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable
        {
            return Comparer<T>.Default.Compare(value, min) >= 0 && Comparer<T>.Default.Compare(value, max) <= 0;
        }        
    }
}