using System.Collections.Generic;
using System.Linq;

namespace Dot.LoadBalance.Weight
{
    public abstract class WeightCalculatorBase<T> : IWeightCalculator<T>
    {
        public int Calculate(T item)
        {
            var weight = this.DoCalculate(item);
            return weight < 1 ? 1 : weight;
        }
        public virtual List<int> Calculate(List<T> items)
        {
            return items.Select(item => this.Calculate(item)).ToList();
        }
        protected abstract int DoCalculate(T item);
    }
}