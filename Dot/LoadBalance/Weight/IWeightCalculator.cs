using System.Collections.Generic;

namespace Dot.LoadBalance.Weight
{
    /// <summary>
    /// 权重计算器
    /// </summary>
    public interface IWeightCalculator<T>
    {
        /// <summary>
        /// 计算 item 的权重
        /// 
        /// 该方法的契约：
        /// 1. 必须保证权重的最小值为1
        /// </summary>
        int Calculate(T item);


        List<int> Calculate(List<T> items);
    }
}