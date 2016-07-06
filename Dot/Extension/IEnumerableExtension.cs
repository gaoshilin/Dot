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
            Ensure.NotNull(items, "items");
            Ensure.NotNull(action, "action");

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

        /// <summary>
        /// 获取枚举数左侧的元素，items 不能为 null，count 必须大于等于 0
        /// </summary>
        public static IEnumerable<T> Left<T>(this IEnumerable<T> items, int count)
        {
            Ensure.NotNull(items, "items");
            Ensure.GreaterOrEqual(count, 0, "count");

            var length = Math.Min(count, items.Count());
            for (int i = 0; i < length; i++)
                yield return items.ElementAt(i);
        }

        /// <summary>
        /// 获取枚举数右侧的元素，items 不能为 null，count 必须大于等于 0
        /// </summary>
        public static IEnumerable<T> Right<T>(this IEnumerable<T> items, int count)
        {
            Ensure.NotNull(items, "items");
            Ensure.GreaterOrEqual(count, 0, "count");

            var skip = Math.Max(0, items.Count() - count);
            return items.Skip(skip).Take(count);
        }

        /// <summary>
        /// 获取枚举数指定下标范围内的元素，items 不能为 null，begin 不能小于0，end 不能小于 begin
        /// </summary>
        public static IEnumerable<T> Between<T>(this IEnumerable<T> items, int begin, int end)
        {
            Ensure.NotNull(items, "items");
            Ensure.GreaterOrEqual(begin, 0, "begin");
            Ensure.GreaterOrEqual(end, begin, "end");

            var count = Math.Min((end - begin + 1), (items.Count() - begin));
            return Enumerable.Range(begin, count).Select(i => items.ElementAt(i));
        }

        /// <summary>
        /// 将枚举数拆分成指定数量的列表，每个列表的元素数量取决于 IEnumerableSplitStrategy
        /// </summary>
        public static List<List<T>> Split<T>(this IEnumerable<T> items, int chunkCount, IEnumerableSplitStrategy strategy = IEnumerableSplitStrategy.None)
        {
            Ensure.NotNull(items, "items");
            Ensure.Greater(chunkCount, 0, "chunkCount");

            if (chunkCount == 1)
                return new List<List<T>> { items.ToList() };

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

        /// <summary>
        /// 新生成一个乱序枚举数
        /// </summary>
        public static IEnumerable<T> Disorder<T>(this IEnumerable<T> items)
        {
            Ensure.NotNull(items, "items");
            return items.OrderBy(item => Guid.NewGuid());
        }

        /// <summary>
        /// 判断枚举数中的所有元素是否都相等
        /// </summary>
        public static bool AllEqual<T>(this IEnumerable<T> items) where T : IComparable<T>
        {
            Ensure.NotNull(items, "items");
            return items.Min().CompareTo(items.Max()) == 0;
        }

        /// <summary>
        /// 生成一个新的枚举数，可指定各元素的重复次数
        /// </summary>
        public static IEnumerable<TElement> SelectRepeat<T, TElement>(this IEnumerable<T> items, Func<T, TElement> elementSelector, Func<T, int> repeatSelector)
        {
            Ensure.NotNull(items, "items");
            Ensure.NotNull(elementSelector, "elementSelector");
            Ensure.NotNull(repeatSelector, "repeatSelector");

            var result = new List<TElement>();
            foreach (var item in items)
            {
                var element = elementSelector(item);
                var repeat = repeatSelector(item);
                result.AddRange(Enumerable.Repeat(element, repeat));
            }

            return result;
        }

        /// <summary>
        /// 计算所有数字的最大公约数
        /// </summary>
        public static int GetGreatestCommonDivisor(this IEnumerable<int> numbers)
        {
            Ensure.NotNull(numbers, "numbers");
            return numbers.Aggregate(IntegerUtil.GetGreatestCommonDivisor);
        }

        /// <summary>
        /// 根据枚举数创建字典
        /// </summary>
        /// <param name="items">源枚举数</param>
        /// <param name="keySelector">key selector</param>
        /// <param name="valueSelector">value selector</param>
        /// <param name="forceDistinct">是否排重，默认不排重</param>
        public static Dictionary<TKey, List<TValue>> MapKeyToValues<TSource, TKey, TValue>(this IEnumerable<TSource> items, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector, bool forceDistinct = false)
        {
            Ensure.NotNull(items, "items");
            Ensure.NotNull(keySelector, "keySelector");
            Ensure.NotNull(valueSelector, "valueSelector");

            var map = new Dictionary<TKey, List<TValue>>();
            foreach (var item in items)
            {
                var key = keySelector(item);
                var value = valueSelector(item);

                if (!map.ContainsKey(key))
                    map.Add(key, new List<TValue>());

                if (!forceDistinct || !map[key].Contains(value))
                    map[key].Add(value);
            }

            return map;
        }

        /// <summary>
        /// 将枚举数的各元素转换成字符串后，用指定的分隔符聚合
        /// </summary>
        public static string JoinToString<T>(this IEnumerable<T> items, string separator = ",", Func<T, string> selector = null)
        {
            Ensure.NotNull(items, "items");

            if (selector == null)
                selector = t => t.ToString();

            return string.Join(separator, items.Select(t => selector(t)));
        }

        public static bool AllReferenceNotEqual<T>(this IEnumerable<T> items) where T : class
        {
            var itemCount = items.Count();

            for (int i = 0; i < itemCount - 1; i++)
            {
                var x = items.ElementAt(i);
                for (int j = i + 1; j < itemCount; j++)
                {
                    var y = items.ElementAt(j);
                    if (object.ReferenceEquals(x, y))
                        return false;
                }
            }

            return true;
        }

        public static bool AllReferenceEqual<T>(this IEnumerable<T> items) where T : class
        {
            var itemCount = items.Count();

            for (int i = 0; i < itemCount - 1; i++)
            {
                var x = items.ElementAt(i);
                for (int j = i + 1; j < itemCount; j++)
                {
                    var y = items.ElementAt(j);
                    if (!object.ReferenceEquals(x, y))
                        return false;
                }
            }

            return true;
        }
    }

    public enum IEnumerableSplitStrategy
    {
        None,
        MaximalAverage
    }
}