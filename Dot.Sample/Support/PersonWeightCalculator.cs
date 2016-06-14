using Dot.LoadBalance.Weight;

namespace Dot.Sample.Support
{
    /// <summary>
    /// 按年龄计算权重
    /// </summary>
    public class PersonWeightCalculator : WeightCalculatorBase<Person>
    {
        protected override int DoCalculate(Dot.Sample.Support.Person item)
        {
            return item.Weight;
        }
    }
}