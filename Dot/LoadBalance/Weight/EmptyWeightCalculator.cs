namespace Dot.LoadBalance.Weight
{
    public class EmptyWeightCalculator<T> : WeightCalculatorBase<T>
    {
        protected override int DoCalculate(T item)
        {
            return 1;
        }
    }
}