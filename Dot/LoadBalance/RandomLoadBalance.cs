using System;
using System.Collections.Generic;
using System.Linq;
using Dot.LoadBalance.Weight;

namespace Dot.LoadBalance
{
    public class RandomLoadBalance : LoadBalanceBase
    {
        private Random _random = new Random();

        protected override T DoEqualSelect<T>(List<T> equalItems, string key)
        {
            var index = _random.Next(equalItems.Count);
            return equalItems.ElementAt(index); 
        }
    }
}