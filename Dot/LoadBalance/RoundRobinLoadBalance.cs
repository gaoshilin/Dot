using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dot.LoadBalance.Weight;
using Dot.Threading.Atomic;
using Dot.Util;

namespace Dot.LoadBalance
{
    public class RoundRobinLoadBalance : LoadBalanceBase
    {
        protected ConcurrentDictionary<string, AtomicInteger> _sequences = new ConcurrentDictionary<string, AtomicInteger>();

        protected override T DoSelect<T>(IWeightCalculator<T> calculator, List<T> items, string key)
        {
            Ensure.NotNullOrEmpty(key, "key");
            return base.DoSelect(calculator, items, key);
        }

        protected override T DoEqualSelect<T>(List<T> equalItems, string key)
        {
            var sequence = _sequences.GetOrAdd(key, new AtomicInteger(0));
            return equalItems.ElementAt(sequence.GetThenIncrement() % equalItems.Count);
        }
    }
}