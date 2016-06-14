using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.LoadBalance;
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