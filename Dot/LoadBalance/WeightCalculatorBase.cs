namespace Dot.LoadBalance
{
    public abstract class WeightCalculatorBase<T> : IWeightCalculator<T>
    {
        public int Calculate(T item)
        {
            var weight = this.DoCalculate(item);
            return weight < 1 ? 1 : weight;
        }

        protected abstract int DoCalculate(T item);
    }
}