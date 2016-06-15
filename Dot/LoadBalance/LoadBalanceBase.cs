using System;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;
using Dot.LoadBalance.Weight;

namespace Dot.LoadBalance
{
    public abstract class LoadBalanceBase : ILoadBalance
    {
        public T Select<T>(IWeightCalculator<T> calculator, List<T> items, string key = "")
        {
            if (calculator == null)
                throw new ArgumentNullException("calculator", "calculator can not be null.");
            if (items == null || !items.Any())
                throw new ArgumentNullException("items", "items can not be null or empty.");
            if (items.Count == 1)
                return items.ElementAt(0);

            return this.DoSelect(calculator, items, key);
        }

        protected virtual T DoSelect<T>(IWeightCalculator<T> calculator, List<T> items, string key)
        {
            var weights = calculator.Calculate(items);
            if (weights.AllEqual())
                return this.DoSelectEqual(items, key);
            else
                return this.DoSelectWeight(items, key, weights);
        }

        /// <summary>
        /// 均等负载
        /// </summary>
        protected abstract T DoSelectEqual<T>(List<T> equalItems, string key);

        /// <summary>
        /// 加权负载
        /// </summary>
        protected virtual T DoSelectWeight<T>(List<T> weightItems, string key, List<int> weights)
        {
            if (weights.Count != weightItems.Count)
                throw new ArgumentException(string.Format("count of weights[{0}] must be equal count of weightItems[{1}]", weights.Count, weightItems.Count), "weights");

            var indexes = Enumerable.Range(0, weightItems.Count);
            var equalItems = indexes.SelectRepeat(i => weightItems.ElementAt(i), i => weights.ElementAt(i)).ToList();

            return this.DoSelectEqual(equalItems, key);
        }
    }
}