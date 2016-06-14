using Dot.LoadBalance;

namespace Dot.Sample.Support
{
    /// <summary>
    /// 按年龄计算权重
    /// </summary>
    public class PersonWeightCalculator : IWeightCalculator<Person>
    {
        public int Calculate(Person item)
        {
            return item.Weight;
        }
    }
}