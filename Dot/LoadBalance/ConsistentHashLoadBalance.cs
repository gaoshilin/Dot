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

        protected override T DoEqualSelect<T>(List<T> items, string key)
        {
            var itemsHash = items.GetHashCode().ToString();
            ConsistentHash<object> selector;
            if (!_selectors.TryGetValue(itemsHash, out selector))
            {
                var nodes = items.Select(i => i as object).ToList();
                selector = new ConsistentHash<object>(nodes);
                _selectors.TryAdd(itemsHash, selector);
            }

            return (T)selector.GetNode(key);
        }

        protected override T DoWeightedSelect<T>(List<T> items, string key, List<int> weights)
        {
            var itemsHash = items.GetHashCode().ToString();
            ConsistentHash<object> selector;
            if (!_selectors.TryGetValue(itemsHash, out selector))
            {
                var nodes = items.Select(i => i as object).ToList();
                selector = new ConsistentHash<object>(nodes, weights);
                _selectors.TryAdd(itemsHash, selector);
            }

            return (T)selector.GetNode(key);
        }
    }
}