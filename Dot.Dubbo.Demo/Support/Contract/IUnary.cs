using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Dot.Dubbo.Registery;
using Dot.Dubbo.Registery.ZooKeeper;
using Dot.Dubbo.Rpc;
using Dot.LoadBalance;
using Dot.LoadBalance.Weight;
using Dot.ServiceModel;
using Dot.ServiceModel.Channels;
using Dot.ZooKeeper;

namespace Dot.Dubbo.Demo.Support.Contract
{
    [ServiceContract]
    public interface IUnary
    {
        [OperationContract]
        double Negate(double x);
    }

    public class Unary : IUnary
    {
        public double Negate(double x)
        {
            return -x;
        }
    }

    public class UnaryServiceInvoker : ServiceInvokerBase<IUnary>, IUnary
    {
        public UnaryServiceInvoker(IRegistery registery, string groupPath, ILoadBalance loadBalance, IWeightCalculator<ServiceMetadata> weightCalculator)
            : base(registery, groupPath, loadBalance, weightCalculator)
        {
        }

        public double Negate(double x)
        {
            using (var proxy = base.Open())
            {
                return proxy.Client.Negate(x);
            }
        }

        protected override Binding GetBinding(ServiceMetadata meta)
        {
            var bindingType = Type.GetType(meta.Binding);
            return BindingFactory.Create(bindingType, "unary");
        }
    }
}