using System;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;

namespace Dot.LoadBalance.Weight
{
    public abstract class LimitedWeightCalculator<T> : WeightCalculatorBase<T>
    {
        private int _limit;

        public LimitedWeightCalculator(int limit)
        {
            _limit = (limit < 10) ? 10 : (limit > 100) ? 100 : limit;
        }

        public override List<int> Calculate(List<T> items)
        {
            var originalWeights = base.Calculate(items);
            var totalWeight = originalWeights.Sum();
            var weightPrecents = originalWeights.Select(weight => (double)weight / totalWeight).ToList();

            totalWeight = Math.Min(_limit, totalWeight);
            //var weights = weightPrecents.Select(precent =>
            //{
            //    var weight = precent * totalWeight;
            //    return Math.Max(1, (int)weight);
            //}).ToList();

            var weights = weightPrecents.Select(precent => Math.Max(1, (int)(precent * totalWeight))).ToList();
            var gcd = weights.GetGreatestCommonDivisor();
            weights = weights.Select(weight => weight / gcd).ToList();

            return weights;
        }
    }
}