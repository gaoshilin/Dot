using System.Collections.Generic;
using System.Linq;
using Dot.Extension;
using Dot.LoadBalance.Weight;

namespace Dot.LoadBalance
{
    public abstract class LoadBalanceBase<T> : ILoadBalance<T>
    {
        public IWeightCalculator<T> WeightCalculator { get; protected set; }

        public LoadBalanceBase(IWeightCalculator<T> weightCalculator)
        {
            this.WeightCalculator = weightCalculator;
        }

        public T Select(List<T> items, string key = null)
        {
            if (items.Any() == false)
                return default(T);
            if (items.Count == 1)
                return items.ElementAt(0);
            return this.DoSelect(items, key);
        }
        protected virtual T DoSelect(List<T> items, string key)
        {
            if (this.HasSameWeight(items))
                return this.DoSelectEqual(items, key);
            else
                return this.DoSelectWeight(items, key);
        }
        protected bool HasSameWeight(List<T> items)
        {
            return WeightCalculator.Calculate(items).AllEqual();
        }

        /// <summary>
        /// 均等负载
        /// </summary>
        protected abstract T DoSelectEqual(List<T> equalItems, string key);
        /// <summary>
        /// 加权负载
        /// </summary>
        protected virtual T DoSelectWeight(List<T> weightItems, string key)
        {
            var weights = WeightCalculator.Calculate(weightItems);
            var indexes = Enumerable.Range(0, weightItems.Count);
            var equalItems = indexes.SelectRepeat(i => weightItems.ElementAt(i), i => weights.ElementAt(i)).ToList();

            return this.DoSelectEqual(equalItems, key);
        }
    }
}