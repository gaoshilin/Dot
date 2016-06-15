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
            if (items == null) 
                throw new ArgumentNullException("items", "items can not be null.");
            if (count <= 0) 
                throw new ArgumentException("count must be greater than 1.", "count");

            count = Math.Min(count, items.Count());
            for (int i = 0; i < count; i++)
                yield return items.ElementAt(i);
        }

        public static IEnumerable<T> Between<T>(this IEnumerable<T> items, int begin, int end)
        {
            if (items == null)
                throw new ArgumentNullException("items", "items can not be null.");
            if (begin < 0)
                throw new ArgumentException("begin must be greater or equal than 0.", "begin");
            if (end < begin)
                throw new ArgumentException("end must be greater or equal than begin", "end");

            var count = Math.Min((end - begin + 1), (items.Count() - begin));
            return Enumerable.Range(begin, count).Select(i => items.ElementAt(i));
        }

        public static List<List<T>> Split<T>(this IEnumerable<T> items, int chunkCount, IEnumerableSplitStrategy strategy = IEnumerableSplitStrategy.None)
        {
            switch (strategy)
            {
                case IEnumerableSplitStrategy.MaximalAverage:
                    return items.SplitMaximalAverage(chunkCount);
                case IEnumerableSplitStrategy.None:
                default:
                    return items.SplitDefault(chunkCount);
            }
        }

        /// <summary>
        /// 将枚举数拆分成指定数量的列表，采用最大平均化的策略
        /// </summary>
        private static List<List<T>> SplitMaximalAverage<T>(this IEnumerable<T> items, int chunkCount)
        {
            var totalCount = items.Count();
            var chunkSize = totalCount / chunkCount;
            var remainder = totalCount % chunkCount;
            var begin = 0;
            var end = 0;

            var result = new List<List<T>>();
            for (int i = 0; i < chunkCount; i++)
            {
                begin = i * chunkSize;
                end = begin + chunkSize - 1;
                result.Add(items.Between(begin, end).ToList());
            }

            for (int i = 0; i < remainder; i++)
            {
                result.ElementAt(i).Add(items.ElementAt(i + end + 1));
            }

            return result;
        }

        /// <summary>
        /// 将枚举数拆分成指定数量的列表，采用将余数添加到尾列表的策略
        /// </summary>
        private static List<List<T>> SplitDefault<T>(this IEnumerable<T> items, int chunkCount)
        {
            var totalCount = items.Count();
            var chunkSize = totalCount / chunkCount;
            var remainder = totalCount % chunkCount;
            var begin = 0;
            var end = 0;

            var result = new List<List<T>>();
            for (int i = 0; i < chunkCount; i++)
            {
                begin = i * chunkSize;
                end = begin + chunkSize - 1;
                if (i == chunkCount - 1)
                    end += remainder;
                result.Add(items.Between(begin, end).ToList());
            }

            return result;
        }

        public static IEnumerable<T> Disorder<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(item => Guid.NewGuid());
        }

        public static bool AllEqual<T>(this IEnumerable<T> items) where T : IComparable<T>
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

    public enum IEnumerableSplitStrategy
    {
        None,
        MaximalAverage
    }
}