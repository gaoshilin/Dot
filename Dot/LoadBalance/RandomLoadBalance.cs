using System;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;

namespace Dot.LoadBalance
{
    public class RandomLoadBalance<T> : LoadBalanceBase<T>
    {
        private Random _random = new Random();

        public RandomLoadBalance(IWeightCalculator<T> weightCalculator)
            : base(weightCalculator)
        {
        }

        protected override T DoSelectEqual(List<T> equalItems, string key)
        {
            var index = _random.Next(equalItems.Count);
            return equalItems.ElementAt(index); 
        }
    }
}