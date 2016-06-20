using System;

namespace Dot.Extension
{
    public static class IComparableExtension
    {
        public static bool GreaterThan<T>(this T param, T comparand)
            where T : IComparable<T>
        {
            return param.CompareTo(comparand) == 1;
        }

        public static bool EqualThan<T>(this T param, T comparand)
            where T : IComparable<T>
        {
            return param.CompareTo(comparand) == 0;
        }

        public static bool SmallerThan<T>(this T param, T comparand)
            where T : IComparable<T>
        {
            return param.CompareTo(comparand) == -1;
        }

        public static bool GreaterOrEqualThan<T>(this T param, T comparand)
            where T : IComparable<T>
        {
            return param.CompareTo(comparand) >= 0;
        }

        public static bool SmallerOrEqualThan<T>(this T param, T comparand)
            where T : IComparable<T>
        {
            return param.CompareTo(comparand) <= 0;
        }
    }
}