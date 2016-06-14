using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dot.Extension
{
    public static class ConcurrentDictionaryExtension
    {
        public static Tuple<bool, TValue> TryGetValue<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            var isSuccess = dict.TryRemove(key, out value);
            return new Tuple<bool, TValue>(isSuccess, value);
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            return dict.TryRemove(key, out value);
        }

        public static bool AddOrReplace<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            return dict.TryAdd(key, value)
                || dict.TryUpdate(key, value, dict[key]);
        }

        public static bool AddOrReplace<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue oldValue, TValue newValue)
        {
            return dict.TryAdd(key, newValue)
                || dict.TryUpdate(key, newValue, oldValue);
        }

        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false)
                dict.Add(key, default(TValue));
            dict[key] = value;
        }
    }
}