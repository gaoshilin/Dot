using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;
using Dot.Threading.Atomic;

namespace Dot.LoadBalance
{
    public class RoundRobinLoadBalance<T> : LoadBalanceBase<T>
    {
        protected ConcurrentDictionary<string, AtomicInteger> _sequences;

        public RoundRobinLoadBalance(IWeightCalculator<T> weightCalculator)
            : base(weightCalculator)
        {
            _sequences = new ConcurrentDictionary<string, AtomicInteger>();
        }

        protected override T DoSelect(List<T> items, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "key is null or empty");

            return base.DoSelect(items, key);
        }
        protected override T DoSelectEqual(List<T> equalItems, string key)
        {
            var sequence = _sequences.GetOrAdd(key, new AtomicInteger(0));
            return equalItems.ElementAt(sequence.GetThenIncrement() % equalItems.Count);
        }
    }
}