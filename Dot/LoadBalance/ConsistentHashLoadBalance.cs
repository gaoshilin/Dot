using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dot.Hash;
using Dot.LoadBalance.Weight;
using Dot.Util;

namespace Dot.LoadBalance
{
    public class ConsistentHashLoadBalance : LoadBalanceBase
    {
        private ConcurrentDictionary<string, ConsistentHash<object>> _selectors = new ConcurrentDictionary<string, ConsistentHash<object>>();

        protected override T DoSelect<T>(IWeightCalculator<T> calculator, List<T> items, string key)
        {
            Ensure.NotNullOrEmpty(key, "key");
            return base.DoSelect(calculator, items, key);
        }

        protected override T DoSelectEqual<T>(List<T> equalItems, string key)
        {
            var itemsHash = equalItems.GetHashCode().ToString();
            ConsistentHash<object> selector;
            if (!_selectors.TryGetValue(itemsHash, out selector))
            {
                selector = new ConsistentHash<object>(equalItems.Select(item => item as object));
                _selectors.TryAdd(itemsHash, selector);
            }

            return (T)selector.GetNode(key);
        }
    }
}