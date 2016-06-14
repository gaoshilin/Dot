using Dot.LoadBalance.Weight;
using Dot.ServiceModel;

namespace Dot.Dubbo.Demo.Support.LoadBalance
{
    public class ServiceMetadataWeightCalculator : LimitedWeightCalculator<ServiceMetadata>
    {
        public ServiceMetadataWeightCalculator(int limit)
            : base(limit)
        {
        }

        protected override int DoCalculate(ServiceMetadata metadata)
        {
            return metadata.Weight;
        }
    }
}