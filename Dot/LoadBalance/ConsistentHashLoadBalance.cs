using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dot.Hash;
using Dot.LoadBalance.Weight;

namespace Dot.LoadBalance
{
    public class ConsistentHashLoadBalance<T> : LoadBalanceBase<T>
    {
        private ConcurrentDictionary<string, ConsistentHash<T>> _selectors;

        public ConsistentHashLoadBalance(IWeightCalculator<T> weightCalculator)
            : base(weightCalculator)
        {
            _selectors = new ConcurrentDictionary<string, ConsistentHash<T>>();
        }

        protected override T DoSelect(List<T> items, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "key is null or empty");

            return base.DoSelect(items, key);
        }

        protected override T DoSelectEqual(List<T> equalItems, string key)
        {
            var itemsHash = equalItems.GetHashCode().ToString();
            ConsistentHash<T> selector;
            if (!_selectors.TryGetValue(itemsHash, out selector))
            {
                selector = new ConsistentHash<T>(equalItems);
                _selectors.TryAdd(itemsHash, selector);
            }

            return selector.GetNode(key);
        }
    }
}