using System.Collections.Generic;

namespace Dot.LoadBalance
{
    /// <summary>
    /// 负载均衡器
    /// </summary>
    public interface ILoadBalance<T>
    {
        IWeightCalculator<T> WeightCalculator { get; }

        T Select(List<T> items, string key = "");
    }
}