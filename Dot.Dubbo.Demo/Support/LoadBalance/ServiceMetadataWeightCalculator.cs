using Dot.LoadBalance.Weight;
using Dot.ServiceModel;

namespace Dot.Dubbo.Demo.Support.LoadBalance
{
    public class ServiceMetadataWeightCalculator : WeightCalculatorBase<ServiceMetadata>
    {
        protected override int DoCalculate(ServiceMetadata metadata)
        {
            return metadata.Weight;
        }
    }
}