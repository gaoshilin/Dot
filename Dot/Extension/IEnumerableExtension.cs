using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dot.Util;

namespace Dot.Extension
{
    public static class IEnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action, int intervalMs = 0)
        {
            if (intervalMs <= 0)
            {
                items.ToList().ForEach(item => action(item));
            }
            else
            {
                items.ToList().ForEach(item =>
                {
                    action(item);
                    Thread.Sleep(intervalMs);
                });
            }
        }

        public static IEnumerable<T> Left<T>(this IEnumerable<T> items, int count)
        {
            if (items == null) throw new ArgumentNullException("items", "items == null");
            if (count <= 0) throw new ArgumentException("length less or equal 0.", "length");

            if (count > items.Count())
                count = items.Count();

            for (int i = 0; i < count; i++)
                yield return items.ElementAt(i);
        }

        public static bool TryRemove<T>(this List<T> items, T item)
        {
            try
            {
                return items.Remove(item);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 乱序
        /// </summary>
        public static IEnumerable<T> Disorder<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(item => Guid.NewGuid());
        }

        public static bool AllEqual<T>(this IEnumerable<T> items)
            where T : IComparable<T>
        {
            return items.Min().CompareTo(items.Max()) == 0;
        }

        public static IEnumerable<TElement> SelectRepeat<T, TElement>(this IEnumerable<T> items, Func<T, TElement> elementSelector, Func<T, int> repeatSelector)
        {
            var result = new List<TElement>();
            foreach (var item in items)
            {
                var element = elementSelector(item);
                var repeat = repeatSelector(item);
                result.AddRange(Enumerable.Repeat(element, repeat));
            }

            return result;
        }

        public static int GetGreatestCommonDivisor(this IEnumerable<int> numbers)
        {
            return numbers.Aggregate(IntegerUtil.GetGreatestCommonDivisor);
        }
    }
}