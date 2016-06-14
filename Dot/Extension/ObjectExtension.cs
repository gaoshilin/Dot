using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dot.Extension
{
    public static class ObjectExtension
    {
        public static bool In<T>(this T item, params T[] range)
        {
            return range.Contains(item);
        }

        public static bool In<T>(this T item, IEnumerable<T> range)
        {
            return range.Contains(item);
        }

        public static bool NotIn<T>(this T item, params T[] range)
        {
            return !In(item, range);
        }

        public static bool NotIn<T>(this T item, IEnumerable<T> range)
        {
            return !In(item, range);
        }

        public static List<PropertyInfo> GetProperties(this object obj)
        {
            if (obj == null)
                return new List<PropertyInfo>();

            return obj.GetType()
                      .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                      .ToList();
        }

        public static T ChangeType<T>(this object value)
            where T : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}